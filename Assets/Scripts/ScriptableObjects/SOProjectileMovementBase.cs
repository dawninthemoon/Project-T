using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SOProjectileMovementBase : ScriptableObject, IMoveBehaviour
{
    public abstract void ExecuteMove(Transform transform, params string[] parameters);
}