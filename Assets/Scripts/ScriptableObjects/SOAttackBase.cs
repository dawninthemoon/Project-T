using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SOAttackBase : ScriptableObject, IAttackBehaviour
{
    public abstract void ExecuteAttack();
}
