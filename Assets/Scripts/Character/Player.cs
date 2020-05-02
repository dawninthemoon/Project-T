﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private PlayerRenderer _playerRenderer;
    private PlayerAttack _playerAttack;
    private bool _attackRequested;

    public override void Initialize() {
        base.Initialize();

        _playerRenderer = GetComponent<PlayerRenderer>();
        _playerAttack = GetComponent<PlayerAttack>();

        _playerAttack.Initialize();
        _playerRenderer.Initialize();
    }

    public void Progress() {
        _playerAttack.Progress(_attackRequested);
    }

    protected override void CalculateVelocity() {
        base.CalculateVelocity();
        if (_attackRequested && _velocity.y > 0f) 
            _velocity.y = 0f;
    }

    protected override void CalculateMoving() {
        base.CalculateMoving();
        _playerRenderer.ApplyAnimation(_input.x, _velocity.y, _jumpRequested);
    }

    public void SetInputX(float horizontal)
    {
        if (_playerAttack.IsInAttackProgress) {
            horizontal = 0f;
        }
        _input.x = horizontal;
    }

    public void SetInputY(float vertical) {
        _input.y = vertical;
    }

    public override void SetJump(bool jumpPressed) {
        if (!_playerAttack.IsInAttackProgress)
            base.SetJump(jumpPressed);
    }

    public override void SetJumpEnd(bool isNotPressed, bool pressedAtLastFrame) {
        if (!_playerAttack.IsInAttackProgress)
            base.SetJumpEnd(isNotPressed, pressedAtLastFrame);
    }

    public void SetAttack(bool attackPressed) => _attackRequested = attackPressed;
}
