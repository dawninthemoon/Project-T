using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerRenderer _playerRenderer;
    private PlayerAttack _playerAttack;
    private bool _attackRequested;
    private bool _throwRequested;
    private GroundMove _controller;
    public Vector2 Velocity { get { return _controller.Velocity;} }

    public void Initialize() {
        _controller = GetComponent<GroundMove>();
        _playerRenderer = GetComponent<PlayerRenderer>();
        _playerAttack = GetComponent<PlayerAttack>();

        var status = GetComponent<TBLPlayerStatus>();
        _controller.Initialize(status.moveSpeed, status.minJumpHeight, status.maxJumpHeight);

        Vector3 throwPos = new Vector3(status.throwXPos, status.throwYPos);
        _playerAttack.Initialize(throwPos);
        _playerRenderer.Initialize();
    }

    public void Progress() {
        bool throwRequested = _throwRequested && (Mathf.Abs(Velocity.y) < Mathf.Epsilon); 
        _playerAttack.Progress(_attackRequested, throwRequested);
    }

    public void FixedProgress() {
        bool jumpRequested = _controller.JumpRequested;
        _controller.FixedProgress();
        _playerAttack.FixedProgress();
        _playerRenderer.ApplyAnimation(_controller.InputX, Velocity.y, jumpRequested);
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

    public void SetThrow(bool throwPressed) => _throwRequested = throwPressed; 

    public void SetAttack(bool attackPressed) => _attackRequested = attackPressed;
}
