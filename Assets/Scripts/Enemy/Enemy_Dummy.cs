using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using DG.Tweening;

public class Enemy_Dummy : EnemyBase
{
    public enum States { Idle, Move, Attack, Hurt, Die }
    private StateMachine<States> _fsm;
    private float _timeAgo;

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

    private void Idle_Enter() {
        _animator.Play("idle", 0, 0f);
    }
    
    private void Hurt_Enter() {
        _timeAgo = 0f;
    }

    private void Hurt_Update() {
        _timeAgo += Time.deltaTime;

        if (_timeAgo >= _knockbackTime && IsHurtStateEnd()) {
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
