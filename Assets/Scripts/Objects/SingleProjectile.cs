using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

[RequireComponent(typeof(TBLProjectileStatus))]
public class SingleProjectile : MonoBehaviour
{
    public int Damage { get; private set; }
    private float _direction;
    private float _speed;
    private Vector2 _offset;
    private Vector2 _size;
    private int _collisionMask;
    private float _maxLifeTime;
    private float _remainLifeTime;

    public void Initalize() {
        var status = GetComponent<TBLProjectileStatus>();
        Damage = status.damage;
        _speed = status.speed;
        _collisionMask = 1 << LayerMask.NameToLayer(status.collisionLayerName);
        _offset = status.hitboxOffset;
        _size = status.hitboxSize;
        _remainLifeTime = _maxLifeTime = status.lifeTime;
    }

    public void Reset(Vector3 position) {
        transform.position = position;
        _remainLifeTime = _maxLifeTime;
    }

    public bool IsLifeTimeEnd() => _remainLifeTime < 0f;

    public bool Progress() {
        _remainLifeTime -= Time.deltaTime;

        transform.position += Vector3.right * _direction * _speed * Time.deltaTime;
        Vector2 cur = transform.position;
        Vector2 offset = _offset.ChangeXPos(_offset.x * _direction);
        var collider = Physics2D.OverlapBox(cur + offset, _size, 0f, _collisionMask);

        return collider != null;
    }

    public void SetDirection(float dir) => _direction = dir;

    private void OnDrawGizmos() {
        var status = GetComponent<TBLProjectileStatus>();
        
        Gizmos.color = Color.green;
        
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
