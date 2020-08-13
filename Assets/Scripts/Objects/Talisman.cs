using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talisman : MonoBehaviour
{
    private int _layerMask;
    public static readonly Vector2 ColliderSize = new Vector2(0.25f, 0.25f);
    public Vector3 _direction;
    public float MoveSpeed { get; private set; }
    public Collider2D StuckedCollider { get; private set; }
    private Vector3 _lastPosition;

    public void Initalize(float dir, float moveSpeed) {
        _layerMask = (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("Enemy"));
        StuckedCollider = null;

        _direction = new Vector3(dir, 0f, 0f);
        MoveSpeed = moveSpeed;
    }

    public void FixedProgress() {
        if (StuckedCollider != null) {
            Vector3 diff = _lastPosition - StuckedCollider.transform.position;
            if (diff.sqrMagnitude > Mathf.Epsilon) {
                _lastPosition = StuckedCollider.transform.position;
                transform.position -= diff;
            }
        }
        else {
            Vector3 moveAmount =  _direction * MoveSpeed * Time.deltaTime;
            transform.position += moveAmount;

            StuckedCollider = Physics2D.OverlapBox(transform.position, ColliderSize, 0f, _layerMask);
            if (StuckedCollider != null) {
                _lastPosition = StuckedCollider.transform.position;
            }
        }
    }
}
