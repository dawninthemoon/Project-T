using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using Aroma;

public class EnemyCaveRatShooterB : EnemyBase
{
    private enum States { Patrol, Chase, ChaseWait, Ready, Attack, AttackWait, Hit, Dead }
    private StateMachine<States> _fsm;
    private Transform _playerTransform;
    private float _targetDirX;
    private int _playerMask;
    private int _obstacleMask;
    [SerializeField] private Vector3 _shotOffset = Vector3.zero;
    private ObjectPool<SingleProjectile> _bombObjectPool;
    private List<SingleProjectile> _activeBombs = new List<SingleProjectile>(10);

    public override void Initialize() {
        base.Initialize();

        _playerMask = 1 << LayerMask.NameToLayer("Player");
        _obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");

        string bombName = "CaveRatShooterBomb";
        _bombObjectPool = new ObjectPool<SingleProjectile>(
            10,
            () => {
                GameObject prefab = AssetLoader.GetInstance().GetPrefab(bombName);
                var bomb = Instantiate(prefab).GetComponent<SingleProjectile>();
                bomb.Initalize();
                return bomb;
            }
        );

        _animator.Initalize("CAVERAT_SHOOTER_", "Patrol", true);
        _fsm = GetComponent<StateMachineRunner>().Initialize<States>(this);
        _fsm.ChangeState(States.Patrol);
    }

    public override bool ReceiveDamage(int amount, float dir) {
        if (_fsm.State == States.Dead) return false;

        if (base.ReceiveDamage(amount, dir))
            _fsm.ChangeState(States.Hit);
        else
            _fsm.ChangeState(States.Dead);

        return true;
    }

    public override void Reset(Vector3 initalPos) {
        base.Reset(initalPos);
        _fsm.ChangeState(States.Patrol);
    }

    public override void Progress() {
        base.Progress();
        for (int i = 0; i < _activeBombs.Count; ++i) {
            var bomb = _activeBombs[i];
            bomb.MoveSelf();
            
            bool isCollisionWithPlayer = bomb.IsCollisionWithPlayer();
            bool isCollisionWithOthers = bomb.IsCollisionWithOthers();

            if (isCollisionWithPlayer || isCollisionWithOthers) {
                bomb.StartHitEffect();
                EffectManager.GetInstance().ShakeCamera(0.2f);
                if (bomb.ExecuteAttack(isCollisionWithPlayer)) {
                    _playerTransform.gameObject.GetComponentNoAlloc<Player>().ReceiveDamage(bomb.Damage);
                }
            }

            if (isCollisionWithPlayer || isCollisionWithOthers|| bomb.IsLifeTimeEnd()) {
                _bombObjectPool.ReturnObject(bomb);
                _activeBombs.RemoveAt(i--);
            }
        }
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

        _playerTransform = DetectPlayer(_moveDetectOffset, _moveDetectSize, _playerMask)?.transform;
        if (_playerTransform != null) {
            if (!WillBeFall(_platformCheckPos.ChangeXPos(-_platformCheckPos.x), _obstacleMask))
                _fsm.ChangeState(States.Chase);
        }
        else if (_timeAgo > 2f) {
            InputX = -InputX;
            ChangeDir(InputX);
            _timeAgo = 0f;
        }
    }

    private void Patrol_Exit() {
        InputX = 0f;
    }

    #endregion

    #region Chase
    private void Chase_Enter() {
        _timeAgo = InputX = 0f;
        _animator.ChangeAnimation("Chase", true);
    }

    private void Chase_Update() {
        if (SetPatrolIfWillBeFall()) return;

        bool isPlayerOut = (DetectPlayer(_moveDetectOffset, _moveDetectSize, _playerMask) == null);
        if (isPlayerOut) {
            if (!_isPlayerOut)
                _timeAgo = 0f;
            if (_timeAgo > 2f)
                _fsm.ChangeState(States.Patrol);
        }
        _isPlayerOut = isPlayerOut;

        if (DetectPlayer(_attackDetectOffset, _attackDetectSize, _playerMask) != null) {
            if (_timeAgo > 0.5f)
                _fsm.ChangeState(States.Ready);
        }
        else {
            InputX = Mathf.Sign((_playerTransform.position - transform.position).x);
            ChangeDir(InputX);
        }
    }

    private void Chase_Exit() {
        InputX = 0f;
    }

    private void ChaseWait_Enter() => _timeAgo = 0f;
    private void ChaseWait_Update() {
        if (_timeAgo > 1f) 
            _fsm.ChangeState(States.Chase);
    }

    #endregion
    
    #region Ready
    private void Ready_Enter() {
        InputX = 0f;
        _targetDirX = Mathf.Sign((_playerTransform.position - transform.position).x);
        ChangeDir(_targetDirX);
        _animator.ChangeAnimation(
            "Ready",
            false,
            () => {
                _fsm.ChangeState(States.Attack);
            }
        );
    }
    #endregion

    #region Attack
    private void Attack_Enter() {
        var bomb = _bombObjectPool.GetObject();
        _activeBombs.Add(bomb);
        
        bomb.Reset(transform.position + _shotOffset);
        bomb.SetDirection(_targetDirX);
        bomb.SetDistance(Vector2.Distance(_playerTransform.position, transform.position));

        _animator.ChangeAnimation(
            "AttackB",
            false,
            () => {
                States nextState = (DetectPlayer(_attackDetectOffset, _attackDetectSize, _playerMask) == null) ? States.ChaseWait : States.AttackWait;
                _fsm.ChangeState(nextState);
            }
        );
    }
    private void AttackWait_Enter() => _timeAgo = 0f;
    private void AttackWait_Update() {
        if (_timeAgo > 1.5f) {
            _fsm.ChangeState(States.Ready);
        }
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
