using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerRenderer _playerRenderer;
    private PlayerAttack _playerAttack;
    private bool _attackRequested;
    private GroundMove _controller;
    public Vector2 Velocity { get { return _controller.Velocity;} }

    public void Initialize() {
        _controller = GetComponent<GroundMove>();
        _playerRenderer = GetComponent<PlayerRenderer>();
        _playerAttack = GetComponent<PlayerAttack>();

        var status = GetComponent<TBLPlayerStatus>();
        _controller.Initialize(status.moveSpeed, status.minJumpHeight, status.maxJumpHeight);
        _playerAttack.Initialize();
        _playerRenderer.Initialize();
    }

    public void Progress() {
        _playerAttack.Progress(_attackRequested);
    }

    public void FixedProgress() {
        _controller.FixedProgress();
        _playerRenderer.ApplyAnimation(_controller.InputX, Velocity.y, _controller.JumpRequested);
    }

    public void SetInputX(float horizontal)
    {
        if (_playerAttack.IsInAttackProgress) {
            horizontal = 0f;
        }
        _controller.InputX = horizontal;
    }

    public void SetInputY(float vertical) {
        _controller.InputY = vertical;
    }

    public void SetJump(bool jumpPressed) {
        if (!_playerAttack.IsInAttackProgress)
            _controller.SetJump(jumpPressed);
        else
            _controller.SetJumpEnd(true, true);   
    }

    public void SetJumpEnd(bool isNotPressed, bool pressedAtLastFrame) {
        if (!_playerAttack.IsInAttackProgress)
            _controller.SetJumpEnd(isNotPressed, pressedAtLastFrame);
    }

    public void SetAttack(bool attackPressed) => _attackRequested = attackPressed;
}
