using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class Enemy_Dummy : EnemyBase
{
    public enum States { Idle, Move, Attack, Hurt, Die }
    private StateMachine<States> _fsm;
    private float _timeAgo;
    private const float HurtEndTime = 0.4f;

    private void Start() {
        Initalize();
    }

    public override void Initalize() {
        base.Initalize();
        _fsm = GetComponent<StateMachineRunner>().Initialize<States>(this);
        _fsm.ChangeState(States.Idle);
    }

    public override void GetDamage(int amount) {
        // _hp -= amount;
        //if (_hp <= 0) {
        //    _fsm.ChangeState(State.Die);
        //}
        _fsm.ChangeState(States.Hurt);
        _animator.Play("hurt", 0, 0f);
    }

    private void Idle_Enter() {
        _animator.Play("idle", 0, 0f);
    }
    
    private void Hurt_Enter() {
        _timeAgo = 0f;
    }

    private void Hurt_Update() {
        _timeAgo += Time.deltaTime;

        if (IsHurtStateEnd()) {
            _fsm.ChangeState(States.Idle);
        }
    }

    private void Die_Enter() {
        _animator.Play("die");
    }

    public bool IsHurtStateEnd() {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return (stateInfo.IsName("hurt") && stateInfo.normalizedTime >= 1f);
    }
}
