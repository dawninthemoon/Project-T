using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    [SerializeField] private int _horizontalRayCount = 4;
    [SerializeField] private int _verticalRayCount = 4;
    [SerializeField] private LayerMask _collisionMask;

    private float _horizontalRaySpacing;
    private float _verticalRaySpacing;

    private const float _skinWidth = 0.015f;

    private BoxCollider2D _collider;
    private RaycastOrigins _raycastOrigins;

    public CollisionInfo _collisions;
    public CollisionInfo Collisions { get { return _collisions; } }

    private void Start()
    {
        Initalize();
    }

    private void Initalize()
    {
        _collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();

        _collisions.Reset();

        if (velocity.x != 0f)
            HorizontalCollisions(ref velocity);
        if (velocity.y != 0f)
            VerticalCollisions(ref velocity);

        transform.Translate(velocity);
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

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(_skinWidth * -2f);

        _raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        _raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        _raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        _raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(_skinWidth * -2f);

        _horizontalRayCount = Mathf.Max(_horizontalRayCount, 2);
        _verticalRayCount = Mathf.Max(_verticalRayCount, 2);

        _horizontalRaySpacing = bounds.size.y / (_horizontalRayCount - 1);
        _verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
    }

    private struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo 
    {
        public bool above, bellow;
        public bool left, right;

        public void Reset()
        {
            above = bellow = false;
            left = right = false;
        }
    }
}
