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
        var status = GetComponent<TBLEnemyStatus>();
        base.Initialize(status.moveSpeed, status.minJumpHeight, status.maxJumpHeight);
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

    protected bool EnableHitbox(Vector2[] points, int layerMask) {
        Vector2 current = transform.position;
        var player = Physics2D.OverlapArea(current + points[0], current + points[1], layerMask)?.GetComponent<Player>();
        player?.ReceiveDamage(10);
        return player != null;
    }

    public virtual bool ReceiveDamage(int damage, float dir) {
        _hp -= damage;
        
        StartKnockback(damage / 30f * dir);
        StartFlash();
        //_animator.Play("hurt", 0, 0f);

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

    protected virtual void OnDrawGizmos() {
        Vector2 position = transform.position;
        var status = GetComponent<TBLEnemyStatus>();
        float dirX = transform.localScale.x;

        DrawLine(status.platformCheckPos, status.platformCheckPos + Vector2.down * 0.2f);

        var movePoint2 = new Vector2(status.MoveDetectStart.x, status.MoveDetectEnd.y);
        var movePoint3 = new Vector2(status.MoveDetectEnd.x, status.MoveDetectStart.y);

        Gizmos.color = Color.green;
        DrawLine(status.MoveDetectStart, movePoint2);
        DrawLine(status.MoveDetectStart, movePoint3);
        DrawLine(movePoint3, status.MoveDetectEnd);
        DrawLine(movePoint2, status.MoveDetectEnd);
        
        var attackPoint2 = new Vector2(status.AttackDetectStart.x, status.AttackDetectEnd.y);
        var attackPoint3 = new Vector2(status.AttackDetectEnd.x, status.AttackDetectStart.y);

        Gizmos.color = Color.red;
        DrawLine(status.AttackDetectStart, attackPoint2);
        DrawLine(status.AttackDetectStart, attackPoint3);
        DrawLine(attackPoint3, status.AttackDetectEnd);
        DrawLine(attackPoint2, status.AttackDetectEnd);

        void DrawLine(Vector2 point1, Vector2 point2) {
            point1.x *= dirX; point2.x *= dirX;
            point1 += position; point2 += position;
            Gizmos.DrawLine(point1, point2);
        }
    }
}
