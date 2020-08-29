using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using Aroma;

public class EnemyCaveRatShooterB : EnemyBase
{
    private enum States { Patrol, Chase, Ready, Attack, Hit, Dead }
    private StateMachine<States> _fsm;
    private Transform _playerTransform;
    private float _targetDirX;
    private int _playerMask;
    private int _obstacleMask;

    public override void Initialize() {
        base.Initialize();

        _playerMask = 1 << LayerMask.NameToLayer("Player");
        _obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");

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

        _playerTransform = DetectPlayer(_moveDetectOffset, _moveDetectSize)?.transform;
        if (_playerTransform != null) {
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

        bool isPlayerOut = (DetectPlayer(_moveDetectOffset, _moveDetectSize) == null);
        if (isPlayerOut) {
            if (!_isPlayerOut)
                _timeAgo = 0f;
            if (_timeAgo > 2f)
                _fsm.ChangeState(States.Patrol);
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
    }

    private void ChaseWait_Enter() {
         if (DetectPlayer(_attackDetectOffset, _attackDetectSize) != null) {
            _fsm.ChangeState(States.Ready);
        }
        else {
            _animator.ChangeAnimation("Ready");
        }
    }

    private void ChaseWait_Update() {
        if (_timeAgo > 1f) {
            _fsm.ChangeState(States.Chase);
        }
    }

    #endregion
    
    #region Ready
    private void Ready_Enter() {
        InputX = 0f;
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
        Vector2 position = transform.position;

        float dirX = transform.localScale.x;
        offset = offset.ChangeXPos(offset.x * dirX);
        position += offset;

        Collider2D collider = Physics2D.OverlapBox(position, size, 0f, _playerMask);
        return collider;
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
    }
}
