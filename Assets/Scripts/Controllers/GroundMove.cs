using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

[RequireComponent(typeof(Controller2D))]
public class GroundMove : MonoBehaviour
{
    private static readonly float TimeToJumpApex = 0.4f;
    private static readonly float AccelerationTimeAirborne = 0.2f;
    private static readonly float AccelerationTimeGrounded = 0.1f;

    private float _moveSpeed;
    private float _gravity;
    private float _maxJumpVelocity;
    private float _minJumpVelocity;

    public float InputX { get; set; }  
    public float InputY { get; set;}

    public bool JumpRequested { get; private set;}
    private bool _jumpReleased;
    private float _velocityXSmoothing;

    private Vector2 _velocity;
    public Vector2 Velocity { get { return _velocity; } }

    private Controller2D _controller;

    public virtual void Initialize(float speed, float minJumpHeight, float maxJumpHeight)
    {
        _controller = GetComponent<Controller2D>();
        _controller.Initialize();

        _moveSpeed = speed;
        _gravity = -(2f * maxJumpHeight) / Mathf.Pow(TimeToJumpApex, 2f);
        _maxJumpVelocity = Mathf.Abs(_gravity) * TimeToJumpApex;
        _minJumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(_gravity) * minJumpHeight);
    }

    public void FixedProgress()
    {
        CalculateVelocity();
        CalculateMoving();
        Reset();
    }

    public void CalculateVelocity() {
        float targetVelocityX = InputX * _moveSpeed;
        float smoothTime = _controller.Collisions.bellow ? AccelerationTimeGrounded : AccelerationTimeAirborne;

        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, smoothTime);
        _velocity.y += _gravity * Time.fixedDeltaTime;

        if (JumpRequested)
            _velocity.y = _maxJumpVelocity;
        
        if (_jumpReleased) {
            if (_velocity.y > _minJumpVelocity) {
                _velocity.y = _minJumpVelocity;
            }
        }
    }

    public void CalculateMoving() {
        Vector3 input = new Vector3(InputX, InputY);
        _controller.Move(_velocity * Time.fixedDeltaTime, input);

        var collisions = _controller.Collisions;
        if (collisions.bellow || collisions.above) {
            if (collisions.slidingDownMaxSlope) {
                _velocity.y += _controller.Collisions.slopeNormal.y * -_gravity * Time.fixedDeltaTime;
            }
            else {
                _velocity.y = 0f;
            }
        }
    }

    public virtual void SetJump(bool jumpPressed) {
        if (jumpPressed && _controller.Collisions.bellow) {
            JumpRequested = true;
        }
    }

    public virtual void SetJumpEnd(bool isNotPressed, bool pressedAtLastFrame) {
        if (isNotPressed && pressedAtLastFrame) {
            _jumpReleased = true;
        }
    }

    protected virtual void Reset() {
        JumpRequested = false;
        _jumpReleased = false;
    }
}
