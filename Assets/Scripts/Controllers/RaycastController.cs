﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    [SerializeField] protected float _distBetweenRays = 0.25f;
    protected int _horizontalRayCount = 4;
    protected int _verticalRayCount = 4;

    protected const float _skinWidth = 0.015f;

    protected BoxCollider2D _collider;
    protected RaycastOrigins _raycastOrigins;

    protected float _horizontalRaySpacing;
    protected float _verticalRaySpacing;

    public virtual void Initialize()
    {
        _collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    protected void UpdateRaycastOrigins()
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

        float boundsWidth = bounds.size.x;
        float boudnsHeight = bounds.size.y;

        _horizontalRayCount = Mathf.RoundToInt(boudnsHeight / _distBetweenRays);
        _verticalRayCount = Mathf.RoundToInt(boundsWidth / _distBetweenRays);

        _horizontalRaySpacing = bounds.size.y / (_horizontalRayCount - 1);
        _verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
    }

    protected struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
