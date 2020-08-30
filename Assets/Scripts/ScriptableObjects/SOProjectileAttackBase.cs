using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SOProjectileAttackBase : ScriptableObject, IAttackBehaviour
{
    public abstract bool ExecuteAttack(Vector2 cur, params string[] parameters);
}
