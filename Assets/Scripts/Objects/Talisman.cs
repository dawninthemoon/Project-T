using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

public class Talisman : MonoBehaviour
{
    private int _layerMask;
    public bool Charged { get; private set; }
    public static readonly Vector2 ColliderSize = new Vector2(0.25f, 0.25f);
    public Vector3 _direction;
    public float MoveSpeed { get; private set; }
    public Collider2D HitCollider { get; private set; }

    public void Initalize(float dir, float moveSpeed, bool charged) {
        _layerMask = (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("Enemy"));
        HitCollider = null;

        _direction = new Vector3(dir, 0f, 0f);
        MoveSpeed = moveSpeed;
        Charged = charged;
    }

    public bool MoveSelf() {
        Vector3 moveAmount =  _direction * MoveSpeed * Time.deltaTime;
        transform.position += moveAmount;

        HitCollider = Physics2D.OverlapBox(transform.position, ColliderSize, 0f, _layerMask);
        if (HitCollider != null) {
            return false;
        }
        return true;
    }

    public EnemyBase GetHitEnemy() {
        return HitCollider.GetComponent<EnemyBase>();
    }
}
