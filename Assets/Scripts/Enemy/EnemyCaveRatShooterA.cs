using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using Aroma;

public class EnemyCaveRatShooterA : EnemyBase
{
    private enum States { Patrol, Chase, ChaseWait, Kite, KiteWait, Ready, Attack, Hit, Dead }
    private StateMachine<States> _fsm; 
    private Transform _playerTransform;
    private ObjectPool<SingleProjectile> _arrowObjectPool;
    private List<SingleProjectile> _activeArrows = new List<SingleProjectile>(3);
    [SerializeField] private Vector3 _shotOffset = Vector3.zero;
    private float _targetDirX;
    private float _targetXPos;
    private int _playerMask;
    private int _obstacleMask;

    public override void Initialize() {
        base.Initialize();

        const string arrowName = "CaveRatShooterArrow";
        _arrowObjectPool = new ObjectPool<SingleProjectile>(
            10,
            () => {
                GameObject prefab = AssetLoader.GetInstance().GetPrefab(arrowName);
                var arrow = Instantiate(prefab).GetComponent<SingleProjectile>();
                arrow.Initalize();
                return arrow;
            }
        );

        _playerMask = 1 << LayerMask.NameToLayer("Player");
        _obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");

        _animator.Initalize("CAVERAT_SHOOTER_", "Patrol", true);
        _fsm = GetComponent<StateMachineRunner>().Initialize<States>(this);
        _fsm.ChangeState(States.Patrol);
    }

    public override void Progress() {
        base.Progress();
        for (int i = 0; i < _activeArrows.Count; ++i) {
            var arrow = _activeArrows[i];
            arrow.MoveSelf();
            
            bool isCollisionWithPlayer = arrow.IsCollisionWithPlayer();

            if (isCollisionWithPlayer) {
                _playerTransform.gameObject.GetComponentNoAlloc<Player>().ReceiveDamage(arrow.Damage);
            }
            if (arrow.IsCollisionWithOthers()) {
                arrow.SetProjectileCantMove();
            }
            if (isCollisionWithPlayer || arrow.IsLifeTimeEnd()) {
                if (!arrow.IsLifeTimeEnd())
                    arrow.StartHitEffect();
                _arrowObjectPool.ReturnObject(arrow);
                _activeArrows.RemoveAt(i--);
            }
        }
    }

    public override bool ReceiveDamage(int amount, float dir, bool rigid = true) {
        if (_fsm.State == States.Dead) return false;

        if (base.ReceiveDamage(amount, dir, rigid)) {
            if (rigid)
                _fsm.ChangeState(States.Hit);
        }
        else
            _fsm.ChangeState(States.Dead);

        return true;
    }

    public override void Reset(Vector3 initalPos) {
        base.Reset(initalPos);
        _fsm.ChangeState(States.Patrol);
    }

    #region Patrol
    private void Patrol_Enter() {
        _timeAgo = 0f;
        InputX = (Random.Range(0, 2) == 0) ? -1f : 1f;
        ChangeDir(InputX);
        _animator.ChangeAnimation("Patrol", true);
    }
    private void Patrol_Update() {
        if (WillBeFall(_platformCheckPos, _obstacleMask)) {
            InputX = -InputX;
            ChangeDir(InputX);
            _timeAgo = 0f;
            return;
        }

       var playerTransform = DetectPlayer(_moveDetectOffset, _moveDetectSize, _playerMask)?.transform;
        if (playerTransform != null) {
            _playerTransform = playerTransform;
            if (!WillBeFall(_platformCheckPos.ChangeXPos(-_platformCheckPos.x), _obstacleMask)) {
                _fsm.ChangeState(States.Chase);
            }
        }
        else if (_timeAgo > 2f) {
            InputX = -InputX;
            ChangeDir(InputX);
            _timeAgo = 0f;
        }
    }

    #endregion
    #region Kite
    private void Kite_Enter() {
        _animator.ChangeAnimation("Chase", true);
        InputX = -Mathf.Sign((_playerTransform.position - transform.position).x);
        ChangeDir(InputX);
        _isPlayerOut = false;
    }
    private void Kite_Update() {
        if (WillBeFall(_platformCheckPos, _obstacleMask)) {
            _fsm.ChangeState(States.Chase); 
            return;
        }
        if (DetectPlayer(_attackDetectOffset.ChangeXPos(-_attackDetectOffset.x), _attackDetectSize, _playerMask) == null) {
            _fsm.ChangeState(States.Ready);
        }
    }
    private void KiteWait_Enter() => _timeAgo = 0f;
    private void KiteWait_Update() {
        if (_timeAgo > 0.5f) 
            _fsm.ChangeState(States.Kite);
    }
    #endregion

    #region Chase
    private void Chase_Enter() {
        _timeAgo = 0f;
        
        _targetXPos = _playerTransform.position.x;
        InputX = Mathf.Sign((_playerTransform.position - transform.position).x);
        ChangeDir(InputX);

        _animator.ChangeAnimation("Chase", true);
        
        _isPlayerOut = false;
    }

    private void Chase_Update() {
        if (SetPatrolIfWillBeFall()) return;

        bool isPlayerOut = (DetectPlayer(_moveDetectOffset, _moveDetectSize, _playerMask) == null);
        if (isPlayerOut) {
            if (!_isPlayerOut)
                _timeAgo = 0f;
            if (_timeAgo > 2f) {
                _fsm.ChangeState(States.Patrol);
            }
        }
        _isPlayerOut = isPlayerOut;

        if (DetectPlayer(_attackDetectOffset, _attackDetectSize, _playerMask) != null) {
            if (_timeAgo > 0.5f)
                _fsm.ChangeState(States.Ready);
        }
        else if ((InputX > 0f && transform.position.x > _targetXPos) || (InputX < 0f && transform.position.x < _targetXPos)) {
            _fsm.ChangeState(States.Patrol);
        }
    }

    private void Chase_Exit() {
        InputX = 0f;
        ChangeDir(Mathf.Sign((_playerTransform.position - transform.position).x));
    }
    private void ChaseWait_Enter() => _timeAgo = 0f;
    private void ChaseWait_Update() {
        if (_timeAgo > 0.5f) 
            _fsm.ChangeState(States.Chase);
    }

    #endregion
    
    #region Ready
    private void Ready_Enter() {
        InputX = 0f;
        _targetDirX = Mathf.Sign((_playerTransform.position - transform.position).x);
        ChangeDir(_targetDirX);
        _animator.ChangeAnimation(
            "ReadyA",
            false,
            () => {
             _fsm.ChangeState(States.Attack);
            }
        );
    }
    #endregion

    #region Attack
    private void Attack_Enter() {
        var arrow = _arrowObjectPool.GetObject();
        _activeArrows.Add(arrow);
        arrow.SetDirection(_targetDirX);
        arrow.Reset(transform.position + _shotOffset.ChangeXPos(_shotOffset.x * transform.localScale.x));

        _animator.ChangeAnimation(
            "AttackA",
            false,
            () => {
                States nextState = (DetectPlayer(_attackDetectOffset, _attackDetectSize, _playerMask) == null) ? States.ChaseWait : States.KiteWait;
                _fsm.ChangeState(nextState);
            }
        );
    }
    #endregion
    
    #region Hit
    private void Hit_Enter() {
        InputX = 0f; InputY = 0f;
        _timeAgo = 0f;
        float dir = transform.localScale.x;
        _animator.ChangeAnimation(
            "Hit",
            false,
            () => {
                _fsm.ChangeState(States.Patrol);
                InputX = dir;
                ChangeDir(dir);
            }
        );
    }

    private void Hit_Update() {
        if (_timeAgo >= _knockbackTime) { 
            _fsm.ChangeState(States.Patrol);
        }
    }
    #endregion

    #region Dead
    private void Dead_Enter() {
        InputX = 0f; InputY = 0f;
        _animator.ChangeAnimation("Dead");
    }
    #endregion

    private bool SetPatrolIfWillBeFall() {
        bool willBeFall = false;
        if (WillBeFall(_platformCheckPos, _obstacleMask)) {
            InputX = 0;
            _fsm.ChangeState(States.Patrol);
        }
        return willBeFall;
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
    }
}
