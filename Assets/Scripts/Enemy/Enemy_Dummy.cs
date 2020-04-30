using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using DG.Tweening;

public class Enemy_Dummy : EnemyBase
{
    private static readonly float MoveDetectRadius = 4f;
    private static readonly float AttackDetectRadius = 1f;

    public enum States { Idle, Move, Attack, Hurt, Die }
    private StateMachine<States> _fsm;
    private float _timeAgo;
    private Transform _playerTransform;
    private float _attackDelay = 0.5f;
    private void Start() {
        Initalize();
    }

    public override void Initalize() {
        base.Initalize();
        _fsm = GetComponent<StateMachineRunner>().Initialize<States>(this);
        _fsm.ChangeState(States.Idle);
    }

    public override bool RecieveDamage(int amount, float dir) {
        if (_fsm.State == States.Die) return false;

        if (base.RecieveDamage(amount, dir))
            _fsm.ChangeState(States.Hurt);
        else
            _fsm.ChangeState(States.Die);

        return true;
    }
    #region Idle
    private void Idle_Enter() {
        _animator.Play("idle");
    }

    private void Idle_Update() {
        _timeAgo += Time.deltaTime;
        if (_timeAgo < _attackDelay) return;
        
        Collider2D collider = DetectPlayer(MoveDetectRadius);
        if (collider != null) {
            _playerTransform = collider.transform;
            _fsm.ChangeState(States.Move);
        }
    }
    #endregion

    #region Move
    private void Move_Enter() {
        _animator.Play("move");
        _timeAgo = 0f;

        if (_playerTransform == null)
            _fsm.ChangeState(States.Idle);
    }

    private void Move_Update() {

        if (DetectPlayer(MoveDetectRadius) == null) {
            _fsm.ChangeState(States.Idle);
            return;
        }

        if (DetectPlayer(AttackDetectRadius) != null) {
            _fsm.ChangeState(States.Attack);
        }
        else {
            Vector3 dir = (_playerTransform.position - transform.position);
            dir.x = Mathf.Sign(dir.x); dir.y = 0f;
            transform.position += dir * _moveSpeed * Time.deltaTime;
        }
    }
    #endregion

    #region Attack

    private void Attack_Enter() {
        _animator.Play("attack");
    }

    private void Attack_Update() {
        if (IsAnimationEnd("attack")) {
            _fsm.ChangeState(States.Idle);
        }
    }

    #endregion
    
    #region Hurt
    private void Hurt_Enter() {
        _timeAgo = 0f;
    }

    private void Hurt_Update() {
        _timeAgo += Time.deltaTime;

        if (_timeAgo >= _knockbackTime && IsAnimationEnd("hurt")) {
            _fsm.ChangeState(States.Idle);
        }
    }
    #endregion

    #region Die
    private void Die_Enter() {
        _animator.Play("die");
    }
    #endregion

    public bool IsAnimationEnd(string stateName) {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return (stateInfo.IsName(stateName) &&stateInfo.normalizedTime >= 1f);
    }

    private Collider2D DetectPlayer(float radius) {
        Vector2 position = transform.position;
        Collider2D collider = Physics2D.OverlapCircle(position, radius, _playerMask);
        return collider;
    }
}
