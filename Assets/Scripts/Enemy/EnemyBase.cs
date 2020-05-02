using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class EnemyBase : MonoBehaviour, IPlaceable
{
    [SerializeField] protected float _moveSpeed = 4f;
    [SerializeField] protected string _enemyName = null;
    [SerializeField] protected int _maxHp = 20;
    [SerializeField]  private float _jumpHeight = 5f;

    private static readonly float TimeToJumpApex = 0.4f;
    private static readonly float AccelerationTimeAirborne = 0.2f;
    private static readonly float AccelerationTimeGrounded = 0.1f;

    protected Animator _animator;
    protected SpriteRenderer _renderer;
    protected int _hp;
    private Sequence _flashSequence;
    protected float _knockbackTime = 0.5f;
    protected int _playerMask;
    protected float _requestX;
    protected bool _jumpRequested;
    protected Controller2D _controller;

    private float _gravity;
    private float _jumpVelocity;
    private float _velocityXSmoothing;
    private Vector2 _velocity;

    public virtual void Initalize() {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _controller = GetComponent<Controller2D>();
        _playerMask = 1 << LayerMask.NameToLayer("Character");

        _controller.Initialize();

        _gravity = -(2f * _jumpHeight) / Mathf.Pow(TimeToJumpApex, 2f);
        _jumpVelocity = Mathf.Abs(_gravity) * TimeToJumpApex;

        Setup();
    }

    public virtual void Setup() {
        _hp = _maxHp;
    }

    public void FixedProgress()
    {
        CalculateVelocity();
        CalculateMoving();
    }

    private void CalculateMoving() {
        if (_requestX != 0f)
            transform.localScale = Aroma.VectorUtility.GetScaleVec(_requestX);

        _controller.Move(_velocity * Time.fixedDeltaTime, Vector2.zero);
        _requestX = 0f;

        var collisions = _controller.Collisions;
        if (collisions._bellow || collisions._above) {
            if (collisions._slidingDownMaxSlope) {
                _velocity.y += _controller.Collisions._slopeNormal.y * -_gravity * Time.fixedDeltaTime;
            }
            else {
                _velocity.y = 0f;
            }
        }

        _jumpRequested = false;
    }

    private void CalculateVelocity() {
        float targetVelocityX = _requestX * _moveSpeed;
        float smoothTime = _controller.Collisions._bellow ? AccelerationTimeGrounded : AccelerationTimeAirborne;

        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, smoothTime);
        _velocity.y += _gravity * Time.fixedDeltaTime;

        if (_jumpRequested)
            _velocity.y = _jumpVelocity;
    }

    public virtual void Reset(Vector3 initalPos) {
        gameObject.SetActive(true);
        transform.position = initalPos;
        _hp = _maxHp;
        _velocity = Vector2.zero;
    }

    public virtual bool ReceiveDamage(int damage, float dir) {
        _hp -= damage;
        
        StartKnockback(damage / 30f * dir);
        StartFlash();
        _animator.Play("hurt", 0, 0f);

        return _hp > 0;
    }

    private void StartKnockback(float localX) {
        transform.DOLocalMoveX(localX, _knockbackTime).SetRelative();
    }

    private void StartFlash() {
        if (_flashSequence == null) {
            _flashSequence = DOTween.Sequence();
            _flashSequence
                .SetAutoKill(false)
                .AppendCallback(() => { _renderer.material.SetFloat("_FlashAmount", 1f); })
                .AppendInterval(0.15f)
                .AppendCallback(() => { _renderer.material.SetFloat("_FlashAmount", 0f); });
        }
        else {
            _flashSequence.Restart();
        }
    }
}
