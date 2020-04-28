﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

[RequireComponent(typeof(Controller2D))]
public class Character : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpHeight = 5f;
    
    private static readonly float TimeToJumpApex = 0.4f;
    private static readonly float AccelerationTimeAirborne = 0.2f;
    private static readonly float AccelerationTimeGrounded = 0.1f;

    private float _gravity;
    private float _jumpVelocity;

    private float _inputX;
    private bool _jumpRequested;
    private bool _attackRequested;
    private float _velocityXSmoothing;
    private bool _bellowAtPreviousFrame;

    private Vector3 _velocity;

    private Controller2D _controller;

    private CharacterRenderer _characterRenderer;

    private CharacterAttack _characterAttack;

    public void Initalize()
    {
        _controller = GetComponent<Controller2D>();
        _characterRenderer = GetComponent<CharacterRenderer>();
        _characterAttack = GetComponent<CharacterAttack>();

        _characterAttack.Initalize();
        _characterRenderer.Initalize();
        _controller.Initalize();

        _gravity = -(2f * _jumpHeight) / Mathf.Pow(TimeToJumpApex, 2f);
        _jumpVelocity = Mathf.Abs(_gravity) * TimeToJumpApex;
    }

    public void FixedProgress()
    {
        _characterAttack.Attack(_attackRequested);

        if (!_characterAttack.IsInAttackProgress) {
            CalculateMoving();
        }
    }

    private void CalculateMoving() {
        CalculateVelocity();

        _controller.Move(_velocity * Time.fixedDeltaTime);

        var collisions = _controller.Collisions;
        if (collisions._bellow || collisions._above) {
            if (collisions._slidingDownMaxSlope) {
                _velocity.y += _controller.Collisions._slopeNormal.y * -_gravity * Time.fixedDeltaTime;
            }
            else {
                _velocity.y = 0f;
            }
        }

        _characterRenderer.ApplyAnimation(_inputX, _velocity.y, _jumpRequested);

        _bellowAtPreviousFrame = collisions._bellow;
        _jumpRequested = false;
    }

    private void CalculateVelocity() {
        float targetVelocityX = _inputX * _moveSpeed;
        float smoothTime = _controller.Collisions._bellow ? AccelerationTimeGrounded : AccelerationTimeAirborne;

        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, smoothTime);
        _velocity.y += _gravity * Time.fixedDeltaTime;
    }

    public void SetInputX(float horizontal)
    {
        _inputX = horizontal;
    }

    public void SetJump(bool jumpPressed) {
        if (jumpPressed && _bellowAtPreviousFrame) {
            _jumpRequested = true;
            _velocity.y = _jumpVelocity;
        }
    }
    public void SetAttack(bool attackPressed) => _attackRequested = attackPressed;
}
