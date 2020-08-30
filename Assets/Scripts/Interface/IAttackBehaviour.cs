using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackBehaviour {
    bool ExecuteAttack(Vector2 cur, params string[] parameters);
}