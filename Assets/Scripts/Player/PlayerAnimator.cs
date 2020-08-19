﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using UnityEngine.U2D;

public partial class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private SpriteAtlas _spriteAtlas = null;

    public enum States { 
        Idle, Attack_A, Attack_B, Attack_air, Attack_in, Attack_out, Dead, Evade, Explode, 
        Fail, Hit, Jump, Land_hard, Land_idle, Land_run, Run, Slide, Throw, Throw_air 
    };

    private StateMachine<States> _fsm;
    private SpriteRenderer _renderer;
    private SpriteAtlasAnimator _animator;

    private Player _player;
    private PlayerAttack _playerAttack;

    #region Non-reference Fields
    private Vector2 _direction;
    private bool _jumpRequested;
    #endregion

    public void Initalize() {
        _animator = new SpriteAtlasAnimator();
        _fsm = GetComponent<StateMachineRunner>().Initialize<States>(this);
        _renderer = GetComponent<SpriteRenderer>();
        _player = GetComponent<Player>();
        _playerAttack = GetComponent<PlayerAttack>();

        _animator.Initalize("PLAYER_", "idle", true);
        _fsm.ChangeState(States.Idle);
    }

    public void Progress() {
        _animator.Progress(_renderer, _spriteAtlas);
    }

    public void ApplyAnimation(float dirX, float velocityY, bool requestJump) {
        _direction.x = dirX;
        _direction.y = (velocityY > 0f) ? 1 : ((velocityY < 0f) ? -1 : 0);

        if (requestJump) {
            _jumpRequested = true;
        }

        if (Mathf.Abs(dirX) > Mathf.Epsilon) {
            SetDirection(dirX);
        }
    }

    public void SetDirection(float dirX) {
        Vector3 scaleVector = new Vector3(Mathf.Sign(dirX), 1f, 1f);
        transform.localScale = scaleVector;
    }
}
