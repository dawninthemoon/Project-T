#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
    [SerializeField] private float _maxSlopeAngle = 80f;
    [SerializeField] private LayerMask _collisionMask;
    private Vector2 _requestedInput;
    private static readonly string ThroughPlatformTag = "Through";

    private CollisionInfo _collisions;
    public CollisionInfo Collisions { get { return _collisions; } }

    public override void Initialize() {
        base.Initialize();
        _collisions._faceDir = 1;
    }

    public void Move(Vector3 velocity, bool standingOnPlatform) {
        Move(velocity, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector3 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        
        _collisions.Reset();
        _collisions._movementOld = moveAmount;

        _requestedInput = input;

        if (moveAmount.y < 0f)
            DescendSlope(ref moveAmount);
        if (moveAmount.x != 0f)
            _collisions._faceDir = (int)Mathf.Sign(moveAmount.x);

        HorizontalCollisions(ref moveAmount);
        
        if (moveAmount.y != 0f)
            VerticalCollisions(ref moveAmount);

        transform.Translate(moveAmount);

        if (standingOnPlatform)
            _collisions._bellow = true;
    }

    private void HorizontalCollisions(ref Vector3 moveAmount)
    {
        float directionX = _collisions._faceDir;
        float rayLength = Mathf.Abs(moveAmount.x) + _skinWidth;

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
                if (i == 0 && (slopeAngle <= _maxSlopeAngle)) {
                    if (_collisions._descendingSlope) {
                        _collisions._descendingSlope = false;
                        moveAmount = _collisions._movementOld;
                    }

                    float distanceToSlopeStart = 0f;
                    if (slopeAngle != _collisions._slopeAngleOld) {
                        distanceToSlopeStart = hit.distance - _skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }

                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!_collisions._climbingSlope || (slopeAngle > _maxSlopeAngle)){
                    moveAmount.x = (hit.distance - _skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (_collisions._climbingSlope) {
                        moveAmount.y = Mathf.Tan(_collisions._slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    _collisions._left = directionX == -1f;
                    _collisions._right = directionX == 1f;
                }
            }
        }
    }

    private void VerticalCollisions(ref Vector3 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + _skinWidth;

        for (int i = 0; i < _verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1f) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, _collisionMask);
            Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLength);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag(ThroughPlatformTag)) {
                    if (directionY > 0f || hit.distance == 0f) continue;
                    if (_collisions._fallingThroughPlatform) continue;

                    if (_requestedInput.y < 0f) {
                        _collisions._fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", 0.5f);
                        continue;
                    }
                }

                moveAmount.y = (hit.distance - _skinWidth) * directionY;
                rayLength = hit.distance;

                if (_collisions._climbingSlope) {
                    moveAmount.x = moveAmount.y / Mathf.Tan(_collisions._slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                _collisions._bellow = directionY == -1f;
                _collisions._above = directionY == 1f;
            }
        }

        if (_collisions._climbingSlope) {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + _skinWidth;
            Vector2 rayOrigin 
                = ((directionX == -1f) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight)
                    + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.right * directionX, rayLength, _collisionMask);
            Debug.DrawRay(rayOrigin, Vector3.right * directionX * rayLength);

            if (hit.collider != null) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != _collisions._slopeAngle) {
                    moveAmount.x = (hit.distance - _skinWidth) * directionX;
                    _collisions._slopeAngle = slopeAngle;
                    _collisions._slopeNormal = hit.normal;
                }
            }
        }
    }

    private void ClimbSlope(ref Vector3 moveAmount, float slopeAngle, Vector2 slopeNormal) {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        
        if (moveAmount.y <= climbVelocityY) {
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            moveAmount.y = climbVelocityY;
            _collisions._bellow = true;
            _collisions._climbingSlope = true;
            _collisions._slopeAngle = slopeAngle;
            _collisions._slopeNormal = slopeNormal;
        }
    }

    private void DescendSlope(ref Vector3 moveAmount) {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(_raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + _skinWidth, _collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(_raycastOrigins.bottomRight, Vector3.down, Mathf.Abs(moveAmount.y) + _skinWidth, _collisionMask);

        if (maxSlopeHitLeft ^ maxSlopeHitRight) {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        }

        if (_collisions._slidingDownMaxSlope)
            return;

        float directionX = Mathf.Sign(moveAmount.x);
        Vector2 rayOrigin = (directionX == -1f) ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, _collisionMask);

        if (hit.collider != null) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0f && slopeAngle <= _maxSlopeAngle) {
                if (Mathf.Sign(hit.normal.x) == directionX) {
                    if (hit.distance - _skinWidth <=  Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x)) {
                        float moveDistance = Mathf.Abs(moveAmount.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

                        moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                        moveAmount.y -= descendVelocityY;

                        _collisions._slopeAngle = slopeAngle;
                        _collisions._descendingSlope = true;
                        _collisions._bellow = true;
                        _collisions._slopeNormal = hit.normal;
                    }
                }
            }
        }
    }

    private void SlideDownMaxSlope(RaycastHit2D hit, ref Vector3 moveAmount) {
        if (hit.collider != null) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle >= _maxSlopeAngle) {
                moveAmount.x = hit.normal.x * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);
            
                _collisions._slopeAngle = slopeAngle;
                _collisions._slidingDownMaxSlope = true;
                _collisions._slopeNormal = hit.normal;
            }
        }
    }

    private void ResetFallingThroughPlatform() {
        _collisions._fallingThroughPlatform = false;
    }

    public struct CollisionInfo
    {
        public bool _above, _bellow;
        public bool _left, _right;
        public bool _climbingSlope, _descendingSlope;
        public bool _slidingDownMaxSlope;
        public float _slopeAngle, _slopeAngleOld;
        public Vector3 _movementOld;
        public Vector2 _slopeNormal;
        public bool _fallingThroughPlatform;
        public int _faceDir;

        public void Reset()
        {
            _above = _bellow = false;
            _left = _right = false;
            _climbingSlope = false;
            _slopeNormal = Vector2.zero;
            _descendingSlope = false;
            _slidingDownMaxSlope = false;
            _slopeAngle = 0f;
        }
    }
}
