

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class EnemyBase : GroundMove, IPlaceable
{
    [SerializeField] protected string _enemyName = null;
    [SerializeField] protected int _maxHp = 20;

    protected Animator _animator;
    protected SpriteRenderer _renderer;
    protected int _hp;
    private Sequence _flashSequence;
    protected float _knockbackTime = 0.5f;

    public virtual void Initialize() {
        base.Initialize(0f, 0f, 0f);
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        Setup();
    }

    public virtual void Setup() {
        _hp = _maxHp;
    }

    public virtual void Reset(Vector3 initalPos) {
        gameObject.SetActive(true);
        transform.position = initalPos;
        _hp = _maxHp;
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
