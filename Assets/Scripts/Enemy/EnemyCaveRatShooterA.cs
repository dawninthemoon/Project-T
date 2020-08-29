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
    private ObjectPool<SingleProjectile> _arrowObjjectPool;
    private List<SingleProjectile> _activeArrows = new List<SingleProjectile>(3);
    private float _targetDirX;
    private int _playerMask;
    private int _obstacleMask;

    public override void Initialize() {
        base.Initialize();

        const string arrowName = "CaveRatShooterArrow";
        _arrowObjjectPool = new ObjectPool<SingleProjectile>(
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
            bool hit = arrow.Progress();
            if (hit) {
                _playerTransform.gameObject.GetComponentNoAlloc<Player>().ReceiveDamage(arrow.Damage);
            }
            if (hit || arrow.IsLifeTimeEnd()) {
                _arrowObjjectPool.ReturnObject(arrow);
                _activeArrows.RemoveAt(i--);
            }
        }
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

    #region Patrol
    private void Patrol_Enter() {
        _timeAgo = 0f;
        InputX = (Random.Range(0, 2) == 0) ? -1f : 1f;
        ChangeDir(InputX);
        _animator.ChangeAnimation("Patrol", true);
    }
    private void Patrol_Update() {
        if (WillBeFall()) {
            InputX = -InputX;
            ChangeDir(InputX);
            _timeAgo = 0f;
            return;
        }

       var playerTransform = DetectPlayer(_moveDetectOffset, _moveDetectSize)?.transform;
        if (playerTransform != null) {
            _playerTransform = playerTransform;
            _fsm.ChangeState(States.Chase);
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
        if (WillBeFall()) {
            _fsm.ChangeState(States.Chase); 
            return;
        }
        if (DetectPlayer(-_attackDetectOffset, _attackDetectSize) == null) {
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
        _timeAgo = InputX = 0f;
        _animator.ChangeAnimation("Chase", true);
        _isPlayerOut = false;
    }

    private void Chase_Update() {
        if (SetPatrolIfWillBeFall()) return;

        bool isPlayerOut = (DetectPlayer(_moveDetectOffset, _moveDetectSize) == null);
        if (isPlayerOut) {
            if (!_isPlayerOut)
                _timeAgo = 0f;
            if (_timeAgo > 2f) {
                _fsm.ChangeState(States.Patrol);
            }
        }
        _isPlayerOut = isPlayerOut;

        if (DetectPlayer(_attackDetectOffset, _attackDetectSize) != null) {
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
        InputX = _timeAgo = 0f;
        _targetDirX = Mathf.Sign((_playerTransform.position - transform.position).x);
        ChangeDir(_targetDirX);
        _animator.ChangeAnimation("ReadyA");
    }
    private void Ready_Update() {
        if (_timeAgo > 0.5f) {
            _fsm.ChangeState(States.Attack);
        }
    }
    #endregion

    #region Attack
    private void Attack_Enter() {
        var arrow = _arrowObjjectPool.GetObject();
        _activeArrows.Add(arrow);
        arrow.SetDirection(_targetDirX);
        arrow.Reset(transform.position);

        _animator.ChangeAnimation(
            "AttackA",
            false,
            () => {
                States nextState = (DetectPlayer(_moveDetectOffset, _moveDetectSize) == null) ? States.ChaseWait : States.KiteWait;
                _fsm.ChangeState(nextState);
            }
        );
    }
    #endregion
    
    #region Hit
    private void Hit_Enter() {
        InputX = 0f; InputY = 0f;
        _timeAgo = 0f;
        _animator.ChangeAnimation(
            "Hit",
            false,
            () => {
                
                _fsm.ChangeState(States.Patrol);
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
        if (WillBeFall()) {
            InputX = 0;
            _fsm.ChangeState(States.Patrol);
        }
        return willBeFall;
    }

    private bool WillBeFall() {
        bool willBeFall = false;

        float xpos = _platformCheckPos.x * (transform.localScale.x);
        Vector2 position = (Vector2)transform.position + _platformCheckPos.ChangeXPos(xpos);
        var platform = Physics2D.Raycast(position, Vector2.down, 0.1f, _obstacleMask);

        if ((Mathf.Abs(Velocity.y) < Mathf.Epsilon) && (platform.collider == null)) {
            willBeFall = true;
        }
        return willBeFall;
    }

    private void ChangeDir(float dir) {
        Vector3 scaleVec = Aroma.VectorUtility.GetScaleVec(Mathf.Sign(dir));
        transform.localScale = scaleVec;
    }

    private Collider2D DetectPlayer(Vector2 offset, Vector2 size) {
        float dirX = transform.localScale.x;
        offset = offset.ChangeXPos(offset.x * dirX);
        Vector2 position = (Vector2)transform.position + offset;

        Collider2D collider = Physics2D.OverlapBox(position, size, 0f, _playerMask);
        return collider;
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
    }
}
