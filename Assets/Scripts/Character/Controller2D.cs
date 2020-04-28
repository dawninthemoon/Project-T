#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
    [SerializeField] private float _maxClimbAngle = 50f;
    [SerializeField] private float _maxDescendAngle = 50f;
    [SerializeField] private LayerMask _collisionMask;

    public float Move(Vector3 velocity, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        
        _collisions.Reset();
        _collisions._velocityOld = velocity;

        if (velocity.x < 0f) {
            DescendSlope(ref velocity);
        }
        if (velocity.x != 0f)
            HorizontalCollisions(ref velocity);
        if (velocity.y != 0f)
            VerticalCollisions(ref velocity);

        transform.Translate(velocity);

        if (standingOnPlatform)
        {
            _collisions._bellow = true;
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

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= _maxClimbAngle) {
                    if (_collisions._descendingSlope) {
                        _collisions._descendingSlope = false;
                        velocity = _collisions._velocityOld;
                    }

                    float distanceToSlopeStart = 0f;
                    if (slopeAngle != _collisions._slopeAngleOld) {
                        distanceToSlopeStart = hit.distance - _skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }

                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!_collisions._climbingSlope || slopeAngle > _maxClimbAngle){
                    velocity.x = (hit.distance - _skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (_collisions._climbingSlope) {
                        velocity.y = Mathf.Tan(_collisions._slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    _collisions._left = directionX == -1f;
                    _collisions._right = directionX == 1f;
                }

                velocity.x = (hit.distance - _skinWidth) * directionX;
                rayLength = hit.distance;

                _collisions._left = directionX == -1f;
                _collisions._right = directionX == 1f;
            }
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

                if (_collisions._climbingSlope) {
                    velocity.x = velocity.y / Mathf.Tan(_collisions._slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                _collisions._bellow = directionY == -1f;
                _collisions._above = directionY == 1f;
            }
        }

        if (_collisions._climbingSlope) {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + _skinWidth;
            Vector2 rayOrigin 
                = ((directionX == -1f) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight)
                    + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.right * directionX, rayLength, _collisionMask);

            if (hit.collider != null) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != _collisions._slopeAngle) {
                    velocity.x = (hit.distance - _skinWidth) * directionX;
                    _collisions._slopeAngle = slopeAngle;
                }
            }
        }
    }

    private void ClimbSlope(ref Vector3 velocity, float slopeAngle) {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY) {
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            velocity.y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
            _collisions._bellow = true;
            _collisions._climbingSlope = true;
            _collisions._slopeAngle = slopeAngle;
        }
    }

    private void DescendSlope(ref Vector3 velocity) {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1f) ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, _collisionMask);

        if (hit.collider != null) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= _maxDescendAngle) {
                if (Mathf.Sign(hit.normal.x) == directionX) {
                    if (hit.distance - _skinWidth <= 
                    Mathf.Tan(slopeAngle * Mathf.Deg2Rad * Mathf.Abs(velocity.x))) {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        _collisions._slopeAngle = slopeAngle;
                        _collisions._descendingSlope = true;
                        _collisions._bellow = true;
                    }
                }
            }
        }
    }
}
