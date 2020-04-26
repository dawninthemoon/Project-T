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
        VerticalCollisions(ref velocity);
        transform.Translate(velocity);
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
            }

            Vector2 start = _raycastOrigins.bottomLeft + Vector2.right * _verticalRaySpacing * i;
            Vector2 dir = Vector2.up * -2f;
#if UNITY_EDITOR
            Debug.DrawRay(start, dir, Color.red);
#endif
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
}
