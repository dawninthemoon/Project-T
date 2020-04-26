#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
    [SerializeField] private LayerMask _collisionMask;

    public float Move(Vector3 velocity, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();

        _collisions.Reset();

        if (velocity.x != 0f)
            HorizontalCollisions(ref velocity);
        if (velocity.y != 0f)
            VerticalCollisions(ref velocity);

        transform.Translate(velocity);

        if (standingOnPlatform)
        {
            _collisions.bellow = true;
        }

        return velocity.y;
    }

    private void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + _skinWidth;

        for (int i = 0; i < _horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1f) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (_verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _collisionMask);

            if (hit.collider != null)
            {
                if (hit.distance == 0f)
                    continue;

                velocity.x = (hit.distance - _skinWidth) * directionX;
                rayLength = hit.distance;

                _collisions.left = directionX == -1f;
                _collisions.right = directionX == 1f;
            }

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
        }
    }

    private void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + _skinWidth;

        for (int i = 0; i < _verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1f) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, _collisionMask);

            if (hit.collider != null)
            {
                velocity.y = (hit.distance - _skinWidth) * directionY;
                rayLength = hit.distance;

                _collisions.bellow = directionY == -1f;
                _collisions.above = directionY == 1f;
            }

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
        }
    }
}
