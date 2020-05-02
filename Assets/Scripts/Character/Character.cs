using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

[RequireComponent(typeof(Controller2D))]
public class Character : MonoBehaviour
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

    private Vector2 _input;
    private bool _jumpRequested;
    private bool _jumpReleased;
    private bool _attackRequested;
    private float _velocityXSmoothing;

    private Vector3 _velocity;

    private Controller2D _controller;

    private CharacterRenderer _characterRenderer;

    private CharacterAttack _characterAttack;

    public void Initialize()
    {
        _controller = GetComponent<Controller2D>();
        _characterRenderer = GetComponent<CharacterRenderer>();
        _characterAttack = GetComponent<CharacterAttack>();

        _characterAttack.Initialize();
        _characterRenderer.Initialize();
        _controller.Initialize();

        _gravity = -(2f * _maxJumpHeight) / Mathf.Pow(TimeToJumpApex, 2f);
        _maxJumpVelocity = Mathf.Abs(_gravity) * TimeToJumpApex;
        _minJumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(_gravity) * _minJumpHeight);
    }

    public void Progress() {
        _characterAttack.Progress(_attackRequested);
    }

    public void FixedProgress()
    {
        CalculateVelocity();
        CalculateMoving();
    }

    private void CalculateVelocity() {
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

    private void CalculateMoving() {
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

        _characterRenderer.ApplyAnimation(_input.x, _velocity.y, _jumpRequested);
        _jumpRequested = false;
        _jumpReleased = false;
    }


    public void SetInputX(float horizontal)
    {
        if (!_characterAttack.IsInAttackProgress) {
            _input.x = horizontal;
            return;
        }
        _input.x = 0f;
    }
    public void SetInputY(float vertical) {
        _input.y = vertical;
    }

    public void SetJump(bool jumpPressed) {
        if (!_characterAttack.IsInAttackProgress && jumpPressed && _controller.Collisions._bellow) {
            _jumpRequested = true;
        }
    }

    public void SetJumpEnd(bool isNotPressed, bool pressedAtLastFrame) {
        if (!_characterAttack.IsInAttackProgress && isNotPressed && pressedAtLastFrame) {
            _jumpReleased = true;
        }
    }

    public void SetAttack(bool attackPressed) => _attackRequested = attackPressed;
}
