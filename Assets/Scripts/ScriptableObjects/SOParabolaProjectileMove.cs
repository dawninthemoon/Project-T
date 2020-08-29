using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ProjectileMovement/Parabola")]
public class SOParabolaProjectileMove : SOProjectileMovementBase {
    public float defaultPower = 10f;
    private const float _gravity = 9.8f;
    public override void ExecuteMove(Transform transform, params string[] parameters) {
        float dir = float.Parse(parameters[0]);
        float speed = float.Parse(parameters[1]);
        float timeAgo = float.Parse(parameters[2]);
        float power = defaultPower - _gravity * timeAgo;

        Vector3 moveAmount = Vector3.right * dir * speed;
        moveAmount.y += power;

        transform.position += moveAmount * Time.deltaTime;
    }
}