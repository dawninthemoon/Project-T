﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

[RequireComponent(typeof(TBLProjectileStatus))]
public class SingleProjectile : MonoBehaviour
{
    public int Damage { get; private set; }
    private float _direction = 1f;
    private float _speed;
    private Vector2 _offset;
    private Vector2 _size;
    private int _playerMask;
    private int _otherMask;
    private float _maxLifeTime;
    private float _remainLifeTime;
    private IMoveBehaviour _projectileMoveCallback;

    public void Initalize() {
        var status = GetComponent<TBLProjectileStatus>();
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
    }

    public void Reset(Vector3 position) {
        transform.position = position;
        _remainLifeTime = _maxLifeTime;
    }

    public bool IsLifeTimeEnd() => _remainLifeTime < 0f;

    public void MoveSelf() {
        _remainLifeTime -= Time.deltaTime;
        _projectileMoveCallback?.ExecuteMove(transform, _direction.ToString(), _speed.ToString(), (_maxLifeTime - _remainLifeTime).ToString());
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

    public void SetDirection(float dir) => _direction = dir;

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
