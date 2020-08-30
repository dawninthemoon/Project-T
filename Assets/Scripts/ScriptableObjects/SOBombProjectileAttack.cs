using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

[CreateAssetMenu(menuName = "ScriptableObjects/ProjectileAttack/Bomb")]
public class SOBombProjectileAttack : SOProjectileAttackBase
{
    [SerializeField] private Vector2 _offset = Vector2.zero;
    [SerializeField] private Vector2 _size = Vector2.one;

    public override bool ExecuteAttack(Vector2 cur, params string[] parameters) {
        float dir = float.Parse(parameters[0]);
        int collisionMask = int.Parse(parameters[1]);

        var hit = Physics2D.OverlapBox(cur + _offset.ChangeXPos(_offset.x * dir), _size, 0f, collisionMask);
        Debug.Log(hit != null);
        return hit != null;
    }
}
