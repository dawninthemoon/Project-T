using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

public class Talisman : MonoBehaviour
{
    public enum TalismanType { Normal, Fire, Electric, Ice }
    private int _layerMask;
    public bool Charged { get; private set; }
    public static readonly Vector2 ColliderSize = new Vector2(0.25f, 0.25f);
    public Vector3 _direction;
    public float MoveSpeed { get; private set; }
    public Collider2D HitCollider { get; private set; }
    public TalismanType Type { get; private set; }

    public void Initalize(float dir, float moveSpeed, bool charged, TalismanType type) {
        _layerMask = (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("Enemy"));
        HitCollider = null;

        _direction = new Vector3(dir, 0f, 0f);
        MoveSpeed = moveSpeed;
        Charged = charged;
        Type = type;
    }

    public bool MoveSelf() {
        Vector3 moveAmount =  _direction * MoveSpeed * Time.deltaTime;
        transform.position += moveAmount;

        HitCollider = GetHitCollider();
        if (HitCollider != null) {
            return false;
        }
        return true;
    }

    public Collider2D GetHitCollider(float scale = 1f) {
        return Physics2D.OverlapBox(transform.position, ColliderSize * scale, 0f, _layerMask);
    }

    public EnemyBase GetHitEnemy() {
        return HitCollider.GetComponent<EnemyBase>();
    }
}
