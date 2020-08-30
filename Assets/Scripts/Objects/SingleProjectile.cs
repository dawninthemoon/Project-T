using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Aroma;

[RequireComponent(typeof(TBLProjectileStatus))]
public class SingleProjectile : MonoBehaviour
{
    [SerializeField] private string _hitEffectName = null;
    public int Damage { get; private set; }
    private float _direction = 1f;
    private float _speed;
    private Vector2 _offset;
    private Vector2 _size;
    private int _playerMask;
    private int _otherMask;
    private float _maxLifeTime;
    private float _remainLifeTime;
    private float _distance;
    private bool _canMove;
    private IMoveBehaviour _projectileMoveCallback;
    private IAttackBehaviour _projectileAttackCallback;
    private SpriteAtlasAnimator _animator;
    private SpriteAtlas _atlas;
    private SpriteRenderer _renderer;

    public void Initalize() {
        _renderer = GetComponent<SpriteRenderer>();
        _atlas = Resources.Load<SpriteAtlas>("Atlas/ProjectileAtlas");
 
        var status = GetComponent<TBLProjectileStatus>();

        _animator = new SpriteAtlasAnimator();
        _animator.Initalize("PROJECTILE_" + status.name, "", true);

        _distance = 0f;
        Damage = status.damage;
        _speed = status.speed;
        string[] layers = status.collisionLayerName.Split(':');
        foreach (string layerName in layers) {
            if (layerName.Equals("Player")) 
                _playerMask = 1 << LayerMask.NameToLayer(layerName);
            else
                _otherMask |= (1 << LayerMask.NameToLayer(layerName));
        }
        _offset = status.hitboxOffset;
        _size = status.hitboxSize;
        _remainLifeTime = _maxLifeTime = status.lifeTime;

        _projectileMoveCallback = AssetLoader.GetInstance().GetScriptableObject(status.moveType) as SOProjectileMovementBase;
        _projectileAttackCallback = AssetLoader.GetInstance().GetScriptableObject(status.attackType) as SOProjectileAttackBase;
    }

    public void Reset(Vector3 position) {
        transform.position = position;
        _remainLifeTime = _maxLifeTime;
        _canMove = true;
    }

    public void StartHitEffect() {
        EffectManager.GetInstance().SpawnAndRemove(transform.position, _hitEffectName, transform.localScale.x);
    }

    public bool IsLifeTimeEnd() => _remainLifeTime < 0f;
    
    public void MoveSelf() {
        _remainLifeTime -= Time.deltaTime;
        _animator.Progress(_renderer, _atlas);

        if (_canMove) {
            string direction = _direction.ToString();
            string speed = _speed.ToString();
            string timeAgo = (_maxLifeTime - _remainLifeTime).ToString();
            string distance = _distance.ToString();
            _projectileMoveCallback?.ExecuteMove(transform, direction, speed, timeAgo, distance);
        }
    }

    public bool IsCollisionWithPlayer() {
        Vector2 cur = transform.position;
        Vector2 offset = _offset.ChangeXPos(_offset.x * _direction);
        var collider = Physics2D.OverlapBox(cur + offset, _size, 0f, _playerMask);

        return collider != null;
    }

    public bool IsCollisionWithOthers() {
        Vector2 cur = transform.position;
        Vector2 offset = _offset.ChangeXPos(_offset.x * _direction);
        var collider = Physics2D.OverlapBox(cur + offset, _size, 0f, _otherMask);
        return collider != null;
    }

    public bool ExecuteAttack(bool isCollisionWithPlayer) {
        if (_projectileAttackCallback == null) return isCollisionWithPlayer;

        bool isCollision = _projectileAttackCallback.ExecuteAttack(transform.position, _direction.ToString(), _playerMask.ToString());
        return isCollision;
    }

    public void SetDirection(float dir) => _direction = dir;
    public void SetDistance(float dist) => _distance = dist;
    public void SetProjectileCantMove() => _canMove = false;

    private void OnDrawGizmos() {
        var status = GetComponent<TBLProjectileStatus>();
        
        Gizmos.color = Color.green;
        
        _direction = (_direction == 0f) ? 1f : _direction;
        Vector2 offset = status.hitboxOffset.ChangeXPos(status.hitboxOffset.x * _direction);
        Vector2 cur = (Vector2)transform.position + offset;
        
        Vector2 p1 = cur, p2 = cur;
        p1.x -= status.hitboxSize.x * 0.5f; p1.y += status.hitboxSize.y * 0.5f; 
        p2 += status.hitboxSize * 0.5f;
        Gizmos.DrawLine(p1, p2);

        p2 = p1; p2.y -= status.hitboxSize.y;
        Gizmos.DrawLine(p1, p2);

        p1 = p2; p2.x += status.hitboxSize.x;
        Gizmos.DrawLine(p1, p2);

        p1 = p2; p2.y += status.hitboxSize.y;
        Gizmos.DrawLine(p1, p2);
    }
}
