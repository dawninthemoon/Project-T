using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using Aroma;

[ExecuteInEditMode]
public class EnemyCaveRat : EnemyBase
{
    private Vector2 _moveDetectStart, _moveDetectEnd;
    private Vector2 _attackDetectStart, _attackDetectEnd;
    private Vector2 _platformCheckPos;
    private Vector2[] _bodyAttackHitboxPoints;
    public enum States { Patrol, Track, TrackWait, AttackReady, TackleStraight, TackleParabola, TackleStraightWait, TackleParabolaWait, Hit, Dead }
    private StateMachine<States> _fsm;
    private Transform _playerTransform;
    private float _targetDirX;
    private bool _hasAttackSuccessed;
    [SerializeField] private float _straightTackleFactor = 1f;
    [SerializeField] private float _parabolaTackleFactor = 1f;
    [SerializeField] private float _straightDashTime = 0.8f;
    private int _playerMask, _obstacleMask;

    public override void Initialize() {
        var status = GetComponent<TBLEnemyStatus>();
        _moveDetectStart = status.MoveDetectStart;
        _moveDetectEnd = status.MoveDetectEnd;
        _attackDetectStart = status.AttackDetectStart;
        _attackDetectEnd = status.AttackDetectEnd;
        _platformCheckPos = status.platformCheckPos;

        var collider = GetComponent<BoxCollider2D>();
        _bodyAttackHitboxPoints = new Vector2[2] {
            collider.offset - collider.size * 0.5f,
            collider.offset + collider.size * 0.5f
        };

        base.Initialize();

        _playerMask = 1 << LayerMask.NameToLayer("Player");
        _obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");

        _animator.Initalize("CAVERAT_", "Patrol", true);
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

        _playerTransform = DetectPlayer(_moveDetectStart, _moveDetectEnd)?.transform;
        if (_playerTransform != null) {
            _fsm.ChangeState(States.Track);
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

    #region Track
    private void Track_Enter() {
        _timeAgo = InputX = 0f;
        _animator.ChangeAnimation("Chase", true);
        
        if (_playerTransform == null)
            _fsm.ChangeState(States.Patrol);
    }

    private void Track_Update() {
        if (SetPatrolIfWillBeFall()) return;

        if (DetectPlayer(_moveDetectStart, _moveDetectEnd) == null) {
            if (_timeAgo > 2f)
                _fsm.ChangeState(States.Patrol);
        }

        if (DetectPlayer(_attackDetectStart, _attackDetectEnd) != null) {
            if (_timeAgo > 0.5f)
                _fsm.ChangeState(States.AttackReady);
        }
        else {
            InputX = Mathf.Sign((_playerTransform.position - transform.position).x);
            ChangeDir(InputX);
        }
    }

    private void Track_Exit() {
        InputX = 0f;
    }

    private void TrackWait_Enter() {
         if (DetectPlayer(_attackDetectStart, _attackDetectEnd) != null) {
            _fsm.ChangeState(States.AttackReady);
        }
        else {
            _animator.ChangeAnimation("Ready");
        }
    }

    private void TrackWait_Update() {
        if (_timeAgo > 1f) {
            _fsm.ChangeState(States.Track);
        }
    }

    #endregion

    #region Attack
    private void AttackReady_Enter() {
        _timeAgo = 0f;
        _targetDirX = Mathf.Sign((_playerTransform.position - transform.position).x);
        ChangeDir(_targetDirX);
        _animator.ChangeAnimation("Ready");
    }

    private void AttackReady_Update() {
        if (_timeAgo > 0.5f) {
             var player = _playerTransform.gameObject.GetComponentNoAlloc<Player>();
             States nextState = (Mathf.Abs(player.Velocity.y) < Mathf.Epsilon) ? States.TackleStraight : States.TackleParabola;
             _fsm.ChangeState(nextState);
        }
    }

    private void TackleStraight_Enter() {
        _timeAgo = 0f;
        _animator.ChangeAnimation("Tackle");
        InputX = _targetDirX * _straightTackleFactor;
    }

    private void TackleStraight_Update() {
        if (SetPatrolIfWillBeFall()) return;

        if (_timeAgo > _straightDashTime) {
            _fsm.ChangeState(States.TrackWait);
        }
        else if (EnableHitbox(_bodyAttackHitboxPoints, _playerMask)) {
            States nextState = (Random.Range(0, 10) > 4) ? States.TackleParabolaWait : States.TackleStraightWait;
            _fsm.ChangeState(nextState);
        }
    }

    private void TackleStraight_Exit() {
        _timeAgo = 0f;
        InputX = 0f;
    }

    private void TackleParabola_Enter() {
        _hasAttackSuccessed = false;
        _animator.ChangeAnimation("Tackle");
        _timeAgo = 0f;
        SetJump(true);
        InputX = _targetDirX * _parabolaTackleFactor;
    }

    private void TackleParabola_Update() {
        if (SetPatrolIfWillBeFall()) return;

        if ((_timeAgo > _straightDashTime) && (Mathf.Abs(Velocity.y) < Mathf.Epsilon)) {
            if (_hasAttackSuccessed) {
                States nextState = (Random.Range(0, 10) > 4) ? States.TackleParabolaWait : States.TackleStraightWait;
                _fsm.ChangeState(nextState);
            }
            else {
                _fsm.ChangeState(States.TrackWait);
            }
        }
        else if (EnableHitbox(_bodyAttackHitboxPoints, _playerMask)) {
            _hasAttackSuccessed = true;
        }
    }

    private void TackleParabola_Exit() {
        _timeAgo = 0f;
        InputX = 0f;
    }

    #endregion
    
    #region AttackWait
    private void TackleStraightWait_Enter() {
        _targetDirX = Mathf.Sign((_playerTransform.position - transform.position).x);
        ChangeDir(_targetDirX);
        _animator.ChangeAnimation("Ready");
    }
    private void TackleStraightWait_Update() {
        if (_timeAgo > 2f) {
            _fsm.ChangeState(States.TackleStraight);
        }
    }
    private void TackleParabolaWait_Enter() {
        _targetDirX = Mathf.Sign((_playerTransform.position - transform.position).x);
        ChangeDir(_targetDirX);
        _animator.ChangeAnimation("Ready");
    }
    private void TackleParabolaWait_Update() {
        if (_timeAgo > 2f) {
            _fsm.ChangeState(States.TackleParabola);
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

    private Collider2D DetectPlayer(Vector2 point1, Vector2 point2) {
        Vector2 position = transform.position;

        float dirX = transform.localScale.x;

        point1.x *= dirX; point2.x *= dirX;
        point1 += position; point2 += position;

        Collider2D collider = Physics2D.OverlapArea(point1, point2, _playerMask);
        return collider;
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
    }
}
