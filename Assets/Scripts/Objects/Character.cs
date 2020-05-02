using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

[RequireComponent(typeof(Controller2D))]
public abstract class Character : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _maxJumpHeight = 5f;
    [SerializeField] private float _minJumpHeight = 2f;
    
    private static readonly float TimeToJumpApex = 0.4f;
    private static readonly float AccelerationTimeAirborne = 0.2f;
    private static readonly float AccelerationTimeGrounded = 0.1f;

    private float _gravity;
    private float _maxJumpVelocity;
    private float _minJumpVelocity;

    protected Vector2 _input;
    protected bool _jumpRequested;
    protected bool _jumpReleased;
    private float _velocityXSmoothing;

    protected Vector3 _velocity;

    protected Controller2D _controller;

    public virtual void Initialize()
    {
        _controller = GetComponent<Controller2D>();
        _controller.Initialize();

        _gravity = -(2f * _maxJumpHeight) / Mathf.Pow(TimeToJumpApex, 2f);
        _maxJumpVelocity = Mathf.Abs(_gravity) * TimeToJumpApex;
        _minJumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(_gravity) * _minJumpHeight);
    }

    public void FixedProgress()
    {
        CalculateVelocity();
        CalculateMoving();
        Reset();
    }

    protected virtual void CalculateVelocity() {
        float targetVelocityX = _input.x * _moveSpeed;
        float smoothTime = _controller.Collisions._bellow ? AccelerationTimeGrounded : AccelerationTimeAirborne;

        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, smoothTime);
        _velocity.y += _gravity * Time.fixedDeltaTime;

        if (_jumpRequested)
            _velocity.y = _maxJumpVelocity;
        
        if (_jumpReleased) {
            if (_velocity.y > _minJumpVelocity) {
                _velocity.y = _minJumpVelocity;
            }
        }
    }

    protected virtual void CalculateMoving() {
        _controller.Move(_velocity * Time.fixedDeltaTime, _input);

        var collisions = _controller.Collisions;
        if (collisions._bellow || collisions._above) {
            if (collisions._slidingDownMaxSlope) {
                _velocity.y += _controller.Collisions._slopeNormal.y * -_gravity * Time.fixedDeltaTime;
            }
            else {
                _velocity.y = 0f;
            }
        }
    }

    public virtual void SetJump(bool jumpPressed) {
        if (jumpPressed && _controller.Collisions._bellow) {
            _jumpRequested = true;
        }
    }

    public virtual void SetJumpEnd(bool isNotPressed, bool pressedAtLastFrame) {
        if (isNotPressed && pressedAtLastFrame) {
            _jumpReleased = true;
        }
    }

    protected virtual void Reset() {
        _jumpRequested = false;
        _jumpReleased = false;
    }
}
