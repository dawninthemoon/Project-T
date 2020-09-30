using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.U2D;
using Aroma;

[RequireComponent(typeof(TBLEnemyStatus))]
public abstract class EnemyBase : GroundMove, IPlaceable, IQuadTreeObject
{
    private SpriteAtlas _atlas;
    private int _maxHP;
    protected int _currentHp;
    private Sequence _flashSequence;
    private Sequence _freezeSequence;
    protected float _knockbackTime = 0.5f;
    private SpriteRenderer _renderer;
    protected SpriteAtlasAnimator _animator;
    protected float _timeAgo;
    protected bool _isPlayerOut;
    protected Vector2 _moveDetectOffset, _moveDetectSize;
    protected Vector2 _attackDetectOffset, _attackDetectSize;
    protected Vector2 _platformCheckPos;
    public string EnemyName { get; private set; }
    private Rect _bounds;

    public virtual void Initialize() {
        _atlas = Resources.Load<SpriteAtlas>("Atlas/CharacterAtlas1");
        var status = GetComponent<TBLEnemyStatus>();

        var collider = GetComponent<BoxCollider2D>();
        _bounds = new Rect(collider.offset, collider.size);

        _moveDetectOffset = status.moveDetectOffset;
        _moveDetectSize = status.moveDetectSize;
        _attackDetectOffset = status.attackDetectOffset;
        _attackDetectSize = status.attackDetectSize;
        _platformCheckPos = status.platformCheckPos;

        base.Initialize(status.moveSpeed, status.minJumpHeight, status.maxJumpHeight);
        _maxHP = status.maxHP;
        EnemyName = status.name;

        _renderer = GetComponent<SpriteRenderer>();
        _animator = new SpriteAtlasAnimator();
        Setup();
    }

    public virtual void Setup() {
        _currentHp = _maxHP;
    }

    public virtual void Reset(Vector3 initalPos) {
        gameObject.SetActive(true);
        transform.position = initalPos;
        _currentHp = _maxHP;
    }

    public virtual void Progress() {
        _timeAgo += Time.deltaTime;
        if (_animator != null) {
            _animator.Progress(_renderer, _atlas);
        }
    }

    protected bool EnableHitbox(Vector2[] points, int layerMask) {
        Vector2 current = transform.position;
        var player = Physics2D.OverlapArea(current + points[0], current + points[1], layerMask)?.GetComponent<Player>();
        player?.ReceiveDamage(10);
        return player != null;
    }

    public virtual bool ReceiveDamage(int damage, float dir, bool rigid = true) {
        _currentHp -= damage;
        
        if (rigid) {
            StartKnockback(damage / 30f * dir);
        }
        StartFlash();

        return _currentHp > 0;
    }

    public void StartFreeze(float duration) {
        Color color = new Color(0.31f, 0.76f, 1f);
        if (_freezeSequence == null) {
            _freezeSequence = DOTween.Sequence();
            _freezeSequence
                .SetAutoKill(false)
                .AppendCallback(() => { _renderer.color = color; })
                .AppendInterval(duration)
                .AppendCallback(() => { _renderer.color = Color.white; });
        }
        else {
            _freezeSequence.Restart();
        }
    }

    public Rect GetBounds() {
        Rect newBounds = new Rect(10f, 10f, 1f, 1f);
        //Rect newBounds = new Rect((Vector2)transform.position + _bounds.position, _bounds.size);
        return newBounds;
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

    protected bool WillBeFall(Vector2 platformCheckPos, int obstacleMask) {
        bool willBeFall = false;

        float xpos = platformCheckPos.x * (transform.localScale.x);
        Vector2 position = (Vector2)transform.position + platformCheckPos.ChangeXPos(xpos);
        var platform = Physics2D.Raycast(position, Vector2.down, 0.1f, obstacleMask);

        if ((Mathf.Abs(Velocity.y) < Mathf.Epsilon) && (platform.collider == null)) {
            willBeFall = true;
        }
        return willBeFall;
    }

    protected void ChangeDir(float dir) {
        Vector3 scaleVec = Aroma.VectorUtility.GetScaleVec(Mathf.Sign(dir));
        transform.localScale = scaleVec;
    }

    protected Collider2D DetectPlayer(Vector2 offset, Vector2 size, int playerMask) {
        Vector2 position = transform.position;

        float dirX = transform.localScale.x;
        offset = offset.ChangeXPos(offset.x * dirX);
        position += offset;

        Collider2D collider = Physics2D.OverlapBox(position, size, 0f, playerMask);
        return collider;
    }

    protected virtual void OnDrawGizmos() {
        Vector2 position = transform.position;
        var status = GetComponent<TBLEnemyStatus>();
        float direction = transform.localScale.x;

        Vector2 platformRayOffset = status.platformCheckPos.ChangeXPos(status.platformCheckPos.x * direction);
        Gizmos.DrawLine(position + platformRayOffset, position + platformRayOffset + Vector2.down * 0.1f);

        Gizmos.color = Color.green;
        DrawBox(status.moveDetectOffset, status.moveDetectSize);

        Gizmos.color = Color.red;
        DrawBox(status.attackDetectOffset, status.attackDetectSize);

        void DrawBox(Vector2 offset, Vector2 size) {
            offset = offset.ChangeXPos(offset.x * direction);
            Vector2 cur = (Vector2)transform.position + offset;
            
            Vector2 p1 = cur, p2 = cur;
            p1.x -= size.x * 0.5f; p1.y += size.y * 0.5f; 
            p2 += size * 0.5f;
            Gizmos.DrawLine(p1, p2);

            p2 = p1; p2.y -= size.y;
            Gizmos.DrawLine(p1, p2);

            p1 = p2; p2.x += size.x;
            Gizmos.DrawLine(p1, p2);

            p1 = p2; p2.y += size.y;
            Gizmos.DrawLine(p1, p2);
        }
    }
}
