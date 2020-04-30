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
    private float _knockbackTime = 0.5f;
    private Sequence _flashSequence;

    private void Start() {
        Initalize();
    }

    public override void Initalize() {
        base.Initalize();
        _fsm = GetComponent<StateMachineRunner>().Initialize<States>(this);
        _fsm.ChangeState(States.Idle);
    }

    public override void GetDamage(int amount, float dir) {
        // _hp -= amount;
        //if (_hp <= 0) {
        //    _fsm.ChangeState(State.Die);
        //}
        _fsm.ChangeState(States.Hurt);
        _animator.Play("hurt", 0, 0f);

        float localX = amount / 30f * dir;
        transform.DOLocalMoveX(localX, _knockbackTime).SetRelative();
        StartFlash();
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

    private void StartFlash() {
        if (_flashSequence == null) {
            _flashSequence = DOTween.Sequence();
            _flashSequence
                .SetAutoKill(false)
                .AppendCallback(() => { _renderer.material.SetFloat("_FlashAmount", 1f); })
                .AppendInterval(0.15f)
                .AppendCallback(() => { _renderer.material.SetFloat("_FlashAmount", 0f); });
        }
        else {
            _flashSequence.Restart();
        }
    }
}
