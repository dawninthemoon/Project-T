using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ProjectileMovement/Straight")]
public class SOStraightProjectileMove : SOProjectileMovementBase {
    public override void ExecuteMove(Transform transform, params string[] parameters) {
        float dir = float.Parse(parameters[0]);
        float speed = float.Parse(parameters[1]);

        Vector3 moveAmount = Vector3.right * dir * speed * Time.deltaTime;
        transform.position += moveAmount;
    }
}