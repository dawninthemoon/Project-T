using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

[ExecuteInEditMode]
public class Enemy_Dummy : EnemyBase
{
    private Vector2 MoveDetectStart, MoveDetectEnd;
    private Vector2 AttackDetectStart, AttackDetectEnd;

    public enum States { Patrol, Track, Attack, Hurt, Die }
    private StateMachine<States> _fsm;
    private float _timeAgo;
    private Transform _playerTransform;
    private float _targetTackleX;
    private float _targetDirX;
    private int _playerMask;

    public override void Initialize() {
        var status = GetComponent<TBLEnemyStatus>();
        MoveDetectStart = status.MoveDetectStart;
        MoveDetectEnd = status.MoveDetectEnd;
        AttackDetectStart = status.AttackDetectStart;
        AttackDetectEnd = status.AttackDetectEnd;

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
        _animator.Play("move");
        _timeAgo = 0f;
        InputX = (Random.Range(0, 2) == 0) ? -1f : 1f;
        ChangeDir(InputX);
    }

    private void Patrol_Update() {
        _timeAgo += Time.deltaTime;

        _playerTransform = DetectPlayer(MoveDetectStart, MoveDetectEnd)?.transform;
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
        _animator.Play("move");
        _timeAgo = 0f;

        if (_playerTransform == null)
            _fsm.ChangeState(States.Patrol);
    }

    private void Track_Update() {
        if (DetectPlayer(MoveDetectStart, MoveDetectEnd) == null) {
            _timeAgo += Time.deltaTime;
            if (_timeAgo > 2f)
                _fsm.ChangeState(States.Patrol);
        }
        else {
            _timeAgo = 0f;
        }

        if (DetectPlayer(AttackDetectStart, AttackDetectEnd) != null) {
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
    #endregion

    #region Attack
    private void Attack_Enter() {
        //_animator.Play("attack");

        _targetDirX = Mathf.Sign((_playerTransform.position - transform.position).x);
        ChangeDir(_targetDirX);

        float tackleDistance = 2f;
        _targetTackleX = transform.position.x + _targetDirX * tackleDistance;
    }

    private void Attack_Update() {
        /*if (IsAnimationEnd("attack")) {
            _fsm.ChangeState(States.Idle);
        }*/

        if ((_targetDirX > 0f && (transform.position.x > _targetTackleX)) || (_targetDirX < 0f && (transform.position.x < _targetTackleX))) {
            _fsm.ChangeState(States.Track);
        }
        InputX = _targetDirX;
    }

    private void Attack_Exit() {
        InputX = _targetDirX = _targetTackleX = 0f;
    }

    #endregion
    
    #region Hurt
    private void Hurt_Enter() {
        InputX = 0f; InputY = 0f;
        _timeAgo = 0f;
    }

    private void Hurt_Update() {
        _timeAgo += Time.deltaTime;

        if (_timeAgo >= _knockbackTime && IsAnimationEnd("hurt")) {
            _fsm.ChangeState(States.Patrol);
        }
    }
    #endregion

    #region Die
    private void Die_Enter() {
        InputX = 0f; InputY = 0f;
        _animator.Play("die");
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
        if (Application.isPlaying) return;

        Vector2 position = transform.position;
        var status = GetComponent<TBLEnemyStatus>();

        var movePoint2 = new Vector2(status.MoveDetectStart.x, status.MoveDetectEnd.y);
        var movePoint3 = new Vector2(status.MoveDetectEnd.x, status.MoveDetectStart.y);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(status.MoveDetectStart, movePoint2);
        Gizmos.DrawLine(status.MoveDetectStart, movePoint3);
        Gizmos.DrawLine(movePoint3, status.MoveDetectEnd);
        Gizmos.DrawLine(movePoint2, status.MoveDetectEnd);
        
        var attackPoint2 = new Vector2(status.AttackDetectStart.x, status.AttackDetectEnd.y);
        var attackPoint3 = new Vector2(status.AttackDetectEnd.x, status.AttackDetectStart.y);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(status.AttackDetectStart, attackPoint2);
        Gizmos.DrawLine(status.AttackDetectStart, attackPoint3);
        Gizmos.DrawLine(attackPoint3, status.AttackDetectEnd);
        Gizmos.DrawLine(attackPoint2, status.AttackDetectEnd);
    }
}
