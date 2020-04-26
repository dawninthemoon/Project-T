#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    [SerializeField] private LayerMask _passengerMask;

    public Vector3 _moveVector;

    protected override void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = _moveVector * Time.fixedDeltaTime;

        MovePassengers(velocity);
        transform.Translate(velocity);
    }

    private void MovePassengers(Vector3 velocity)
    {
        HashSet<Transform> movePassengers = new HashSet<Transform>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        if (velocity.y != 0f)
        {
            float rayLength = Mathf.Abs(velocity.y) + _skinWidth;

            for (int i = 0; i < _verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1f) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, _passengerMask);

                if (hit.collider != null)
                {
                    if (!movePassengers.Contains(hit.transform))
                    {
                        movePassengers.Add(hit.transform);

                        float pushX = (directionY == 1f) ? velocity.x : 0f;
                        float pushY = velocity.y - (hit.distance - _skinWidth) * directionY;

                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }
    }
}
