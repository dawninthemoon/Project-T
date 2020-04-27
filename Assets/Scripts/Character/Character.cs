using System.Collections;
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

    private Vector3 _velocity;

    private Controller2D _controller;

    private CharacterRenderer _characterRenderer;

    private CharacterAttack _characterAttack;

    private void Start()
    {
        Initalize();
    }

    private void Initalize()
    {
        _controller = GetComponent<Controller2D>();
        _characterRenderer = GetComponent<CharacterRenderer>();
        _characterAttack = GetComponent<CharacterAttack>();

        _gravity = -(2f * _jumpHeight) / Mathf.Pow(TimeToJumpApex, 2f);
        _jumpVelocity = Mathf.Abs(_gravity) * TimeToJumpApex;
    }

    private void FixedUpdate()
    {
        _characterAttack.Attack(_attackRequested);

        if (!_characterAttack.IsInAttackProgress) {
            CalculateMoving();
        }
    }

    private void CalculateMoving() {
        
        var collisions = _controller.Collisions;
        if (collisions.bellow || collisions.above)
            _velocity.y = 0f;

        _characterRenderer.ApplyAnimation(_inputX, _velocity.y, _jumpRequested);

        if (_jumpRequested && collisions.bellow)
            _velocity.y = _jumpVelocity;

        float targetVelocityX = _inputX * _moveSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, collisions.bellow ? AccelerationTimeGrounded : AccelerationTimeAirborne);

        _velocity.y += _gravity * Time.fixedDeltaTime;
        float appliedVelocityY = _controller.Move(_velocity * Time.fixedDeltaTime);
    }

    public void SetInputX(float horizontal)
    {
        _inputX = horizontal;
    }

    public void SetJump(bool jumpPressed) => _jumpRequested = jumpPressed;
    public void SetAttack(bool attackPressed) => _attackRequested = attackPressed;
}
