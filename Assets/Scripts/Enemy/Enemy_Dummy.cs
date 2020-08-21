using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using Aroma;

[ExecuteInEditMode]
public class Enemy_Dummy : EnemyBase
{
    private Vector2 _moveDetectStart, _moveDetectEnd;
    private Vector2 _attackDetectStart, _attackDetectEnd;
    private Vector2[] _bodyAttackHitboxPoints;
    public enum States { Patrol, Track, TrackWait, Attack, TackleStraight, TackleParabola, TackleStraightWait, TackleParabolaWait, Hurt, Die }
    private StateMachine<States> _fsm;
    private float _timeAgo;
    private Transform _playerTransform;
    private float _targetTackleX;
    private float _targetDirX;
    [SerializeField] private float _straightTackleFactor = 1f;
    [SerializeField] private float _parabolaTackleFactor = 1f;
    private int _playerMask;

    public override void Initialize() {
        var status = GetComponent<TBLEnemyStatus>();
        _moveDetectStart = status.MoveDetectStart;
        _moveDetectEnd = status.MoveDetectEnd;
        _attackDetectStart = status.AttackDetectStart;
        _attackDetectEnd = status.AttackDetectEnd;

        var collider = GetComponent<BoxCollider2D>();
        _bodyAttackHitboxPoints = new Vector2[2] {
            collider.offset - collider.size * 0.5f,
            collider.offset + collider.size * 0.5f
        };

        base.Initialize();
        _playerMask = 1 << LayerMask.NameToLayer("Player");
        _fsm = GetComponent<StateMachineRunner>().Initialize<States>(this);
        _fsm.ChangeState(States.Patrol);
    }

    public override bool ReceiveDamage(int amount, float dir) {
        if (_fsm.State == States.Die) return false;

        if (base.ReceiveDamage(amount, dir))
            _fsm.ChangeState(States.Hurt);
        else
            _fsm.ChangeState(States.Die);

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
    }

    private void Patrol_Update() {
        _timeAgo += Time.deltaTime;

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

        if (_playerTransform == null)
            _fsm.ChangeState(States.Patrol);
    }

    private void Track_Update() {
        if (DetectPlayer(_moveDetectStart, _moveDetectEnd) == null) {
            _timeAgo += Time.deltaTime;
            if (_timeAgo > 2f)
                _fsm.ChangeState(States.Patrol);
        }
        else {
            _timeAgo = 0f;
        }

        if (DetectPlayer(_attackDetectStart, _attackDetectEnd) != null) {
            _fsm.ChangeState(States.Attack);
        }
        else {
            InputX = Mathf.Sign((_playerTransform.position - transform.position).x);
            ChangeDir(InputX);
        }
    }

    private void Track_Exit() {
        InputX = 0f;
    }

    private void TrackWait_Update() {
        _timeAgo += Time.deltaTime;
        if (_timeAgo > 0.5f) {
            _fsm.ChangeState(States.Track);
        }
    }

    #endregion

    #region Attack
    private void Attack_Enter() {
        _targetDirX = Mathf.Sign((_playerTransform.position - transform.position).x);
        ChangeDir(_targetDirX);

        var player = _playerTransform.gameObject.GetComponentNoAlloc<Player>();
        States nextState = (Mathf.Abs(player.Velocity.y) < Mathf.Epsilon) ? States.TackleStraight : States.TackleParabola;
        _fsm.ChangeState(nextState);
    }

    private void TackleStraight_Enter() {
        float tackleDistance = 5f;
        _targetTackleX = transform.position.x + _targetDirX * tackleDistance;
    }

    private void TackleStraight_Update() {
        if ((_targetDirX > 0f && (transform.position.x > _targetTackleX)) || (_targetDirX < 0f && (transform.position.x < _targetTackleX))) {
            _fsm.ChangeState(States.TrackWait);
        }
        else if (EnableHitbox(_bodyAttackHitboxPoints, _playerMask)) {
            States nextState = (Random.Range(0, 10) > 4) ? States.TackleParabolaWait : States.TackleStraightWait;
            _fsm.ChangeState(nextState);
        }
        else { 
            InputX = _targetDirX * _straightTackleFactor;
        }
    }

    private void TackleStraight_Exit() {
        _timeAgo = 0f;
        InputX = 0f;
    }

    private void TackleParabola_Enter() {
        float tackleDistance = 1.5f;
        _targetTackleX = transform.position.x + _targetDirX * tackleDistance;
        SetJump(true);
    }

    private void TackleParabola_Update() {
        if ((_targetDirX > 0f && (transform.position.x > _targetTackleX)) || (_targetDirX < 0f && (transform.position.x < _targetTackleX))) {
            _fsm.ChangeState(States.TrackWait);
        }
        else if (EnableHitbox(_bodyAttackHitboxPoints, _playerMask)) {
            States nextState = (Random.Range(0, 10) > 4) ? States.TackleParabolaWait : States.TackleStraightWait;
            _fsm.ChangeState(nextState);
        }
        else { 
            InputX = _targetDirX * _parabolaTackleFactor;
        }
    }

    private void TackleParabola_Exit() {
        _timeAgo = 0f;
        InputX = 0f;
    }

    #endregion
    
    #region AttackWait


    private void TackleStraightWait_Update() {
        _timeAgo += Time.deltaTime;
        if (_timeAgo > 1f) {
            _fsm.ChangeState(States.TackleStraight);
        }
    }

    private void TackleParabolaWait_Update() {
        _timeAgo += Time.deltaTime;
        if (_timeAgo > 1f) {
            _fsm.ChangeState(States.TackleParabola);
        }
    }

    #endregion

    #region Hurt
    private void Hurt_Enter() {
        InputX = 0f; InputY = 0f;
        _timeAgo = 0f;
    }

    private void Hurt_Update() {
        _timeAgo += Time.deltaTime;

        if (_timeAgo >= _knockbackTime) { //&& IsAnimationEnd("hurt")) {
            _fsm.ChangeState(States.Patrol);
        }
    }
    #endregion

    #region Die
    private void Die_Enter() {
        InputX = 0f; InputY = 0f;
    }
    #endregion
    
    private void ChangeDir(float dir) {
        Vector3 scaleVec = Aroma.VectorUtility.GetScaleVec(Mathf.Sign(dir));
        transform.localScale = scaleVec;
    }

    public bool IsAnimationEnd(string stateName) {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return (stateInfo.IsName(stateName) && stateInfo.normalizedTime >= 1f);
    }

    private Collider2D DetectPlayer(Vector2 point1, Vector2 point2) {
        Vector2 position = transform.position;

        float dirX = transform.localScale.x;

        point1.x *= dirX; point2.x *= dirX;
        point1 += position; point2 += position;

        Collider2D collider = Physics2D.OverlapArea(point1, point2, _playerMask);
        return collider;
    }

    private void OnDrawGizmos() {
        //if (Application.isPlaying) return;

        Vector2 position = transform.position;
        var status = GetComponent<TBLEnemyStatus>();
        float dirX = transform.localScale.x;

        var movePoint2 = new Vector2(status.MoveDetectStart.x, status.MoveDetectEnd.y);
        var movePoint3 = new Vector2(status.MoveDetectEnd.x, status.MoveDetectStart.y);

        Gizmos.color = Color.green;
        DrawLine(status.MoveDetectStart, movePoint2);
        DrawLine(status.MoveDetectStart, movePoint3);
        DrawLine(movePoint3, status.MoveDetectEnd);
        DrawLine(movePoint2, status.MoveDetectEnd);
        
        var attackPoint2 = new Vector2(status.AttackDetectStart.x, status.AttackDetectEnd.y);
        var attackPoint3 = new Vector2(status.AttackDetectEnd.x, status.AttackDetectStart.y);

        Gizmos.color = Color.red;
        DrawLine(status.AttackDetectStart, attackPoint2);
        DrawLine(status.AttackDetectStart, attackPoint3);
        DrawLine(attackPoint3, status.AttackDetectEnd);
        DrawLine(attackPoint2, status.AttackDetectEnd);

        void DrawLine(Vector2 point1, Vector2 point2) {
            point1.x *= dirX; point2.x *= dirX;
            point1 += position; point2 += position;
            Gizmos.DrawLine(point1, point2);
        }
    }
}
