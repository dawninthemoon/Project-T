#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

public class PlatformController : RaycastController, IPlaceable
{
    private LayerMask _passengerMask;
    [SerializeField] private Vector3[] _localWayPoints;
    public Vector3[] LocalWayPoints { get { return _localWayPoints; } }
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed { get { return _moveSpeed; } }
    [SerializeField] private bool _cyclic;
    public bool Cyclic { get { return _cyclic; }}
    [SerializeField] private float _waitTime;
    public float WaitTime { get { return _waitTime; } }
    [SerializeField, Range(0f, 2f)] private float _easeAmount;
    public float EaseAmount { get { return _easeAmount; } }

    private Vector3[] _globalWayPoints;
    private int _fromWaypointIndex;
    private float _percentBetweenWaypoints;
    private float _nextMoveTime;

    private HashSet<Transform> _movePassengers;
    private List<PassengerMovement> _passengerMovement;
    private Dictionary<Transform, Controller2D> _passengerDictionary;

    // Have to alter because this is controller
    private SpriteRenderer _renderer;

    public override void Initialize()
    {
        base.Initialize();

        _passengerMask = LayerMask.NameToLayer("Player");

        _renderer = GetComponent<SpriteRenderer>();
        _movePassengers = new HashSet<Transform>();
        _passengerMovement = new List<PassengerMovement>();
        _passengerDictionary = new Dictionary<Transform, Controller2D>();
    }

    public void Setup(MovingPlatformPoint info) {
        Reset(info.position);
        
        _localWayPoints = info.localWayPoints;
        _moveSpeed = info.moveSpeed;
        _cyclic = info.cyclic;
        _waitTime = info.waitTime;
        _easeAmount = info.easeAmount;
        _renderer.sprite = info.sprite;

        _globalWayPoints = new Vector3[_localWayPoints.Length];
        for (int i = 0; i < _globalWayPoints.Length; i++) {
            _globalWayPoints[i] = transform.position + _localWayPoints[i];
        }
    }

    public void Reset(Vector3 initalPos) {
        transform.position = initalPos;
        _nextMoveTime = 0f;
        _fromWaypointIndex = 0;
        _percentBetweenWaypoints = 0f;
    }

    public void FixedProgress()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();

        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        Physics2D.SyncTransforms();
        MovePassengers(false);
    }

    private Vector3 CalculatePlatformMovement()
    {
        if (Time.time < _nextMoveTime)
            return Vector3.zero;

        int wayPointsLength = _globalWayPoints.Length;
        _fromWaypointIndex %= wayPointsLength;
        int toWaypointIndex = (_fromWaypointIndex + 1) % wayPointsLength; 

        float distanceBetweenWaypoints = Vector3.Distance(_globalWayPoints[_fromWaypointIndex], _globalWayPoints[toWaypointIndex]);
        _percentBetweenWaypoints += Time.fixedDeltaTime * _moveSpeed / distanceBetweenWaypoints;
        _percentBetweenWaypoints = Mathf.Clamp01(_percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = CustomMath.Ease(_percentBetweenWaypoints, _easeAmount);

        Vector3 newPos = Vector3.Lerp(_globalWayPoints[_fromWaypointIndex], _globalWayPoints[toWaypointIndex], easedPercentBetweenWaypoints
            );

        if (_percentBetweenWaypoints >= 1f)
        {
            _percentBetweenWaypoints = 0f;
            _fromWaypointIndex++;
            if (!_cyclic && _fromWaypointIndex >= _globalWayPoints.Length - 1)
            {
                _fromWaypointIndex = 0;
                System.Array.Reverse(_globalWayPoints);
            }

            _nextMoveTime = Time.time + _waitTime;
        }

        return newPos - transform.position;
    }

    private void MovePassengers(bool beforeMovePlatform)
    {
        foreach (PassengerMovement passenger in _passengerMovement)
        {
            if (!_passengerDictionary.ContainsKey(passenger.transform))
            {
                _passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }

            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                _passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    private void CalculatePassengerMovement(Vector3 velocity)
    {
        this._movePassengers.Clear();
        _passengerMovement.Clear();

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

                if (hit.collider != null && hit.distance != 0f)
                {
                    if (!_movePassengers.Contains(hit.transform))
                    {
                        _movePassengers.Add(hit.transform);

                        float pushX = (directionY == 1f) ? velocity.x : 0f;
                        float pushY = velocity.y - (hit.distance - _skinWidth) * directionY;

                        _passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1f, true));
                    }
                }
            }
        }

        if (velocity.x != 0f)
        {
            float rayLength = Mathf.Abs(velocity.x) + _skinWidth;

            for (int i = 0; i < _verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1f) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (_horizontalRaySpacing * i + velocity.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _passengerMask);

                if (hit.collider != null && hit.distance != 0f)
                {
                    if (!_movePassengers.Contains(hit.transform))
                    {
                        _movePassengers.Add(hit.transform);

                        float pushX = velocity.x - (hit.distance - _skinWidth) * directionX;
                        float pushY = -_skinWidth;

                        _passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }

        // Passenger on top of a horizontally or downward moving platform
        if (directionY == -1f || velocity.y == 0f && velocity.x != 0f)
        {
            float rayLength = Mathf.Abs(velocity.y) + _skinWidth;

            for (int i = 0; i < _verticalRayCount; i++)
            {
                Vector2 rayOrigin = _raycastOrigins.topLeft + Vector2.right * (_verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, _passengerMask);

                if (hit.collider != null && hit.distance != 0f)
                {
                    if (!_movePassengers.Contains(hit.transform))
                    {
                        _movePassengers.Add(hit.transform);

                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        _passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        if (_localWayPoints != null)
        {
            Gizmos.color = Color.red;
            float size = 0.2f;

            Color defaultColor = Gizmos.color;

            int pointsCount = _localWayPoints.Length;
            Color[] colors = new Color[4]; colors[0] = Color.red; colors[1] = Color.yellow; colors[2] = Color.green; colors[3] = Color.blue;   

            for (int i = 0; i < pointsCount; i++)
            {
                Color color = colors[i % 4];
                Vector3 globalWayPointPos = (Application.isPlaying) ? _globalWayPoints[i] : _localWayPoints[i] + transform.position;
                
                Vector3 plusMinus = new Vector3(1f, -1f, 0f);
                Vector3 minusPlus = new Vector3(-1f, 1f, 0f);

                Gizmos.color = color;

                Gizmos.DrawLine(globalWayPointPos + minusPlus * size, globalWayPointPos + plusMinus * size);
                Gizmos.DrawLine(globalWayPointPos + Vector3.one * size, globalWayPointPos - Vector3.one * size);

                if (i < pointsCount - 1) {
                    Gizmos.DrawLine(_localWayPoints[i] + transform.position, _localWayPoints[i + 1] + transform.position);
                }
            }

            Gizmos.color = defaultColor;
        }
    }

    private struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform t, Vector3 v, bool standing, bool moved)
        {
            transform = t;
            velocity = v;
            standingOnPlatform = standing;
            moveBeforePlatform = moved;
        }
    }
}
