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
    private float _velocityXSmoothing;

    private Vector3 _velocity;

    private Controller2D _controller;

    private void Start()
    {
        Initalize();
    }

    private void Initalize()
    {
        _controller = GetComponent<Controller2D>();

        _gravity = -(2f * _jumpHeight) / Mathf.Pow(TimeToJumpApex, 2f);
        _jumpVelocity = Mathf.Abs(_gravity) * TimeToJumpApex;
    }

    private void FixedUpdate()
    {
        var collisions = _controller.Collisions;
        if (collisions.bellow || collisions.above)
            _velocity.y = 0f;

        if (_jumpRequested && collisions.bellow)
            _velocity.y = _jumpVelocity;

        float targetVelocityX = _inputX * _moveSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, collisions.bellow ? AccelerationTimeGrounded : AccelerationTimeAirborne);

        _velocity.y += _gravity * Time.fixedDeltaTime;
        _controller.Move(_velocity * Time.fixedDeltaTime);
    }

    public void SetInputX(float horizontal)
    {
        _inputX = horizontal;
    }

    public void SetJump(bool jumpPressed) => _jumpRequested = jumpPressed;
}