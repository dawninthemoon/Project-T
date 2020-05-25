using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPoint {
    public EnemyTypes requestEnemy = EnemyTypes.Enemy_Dummy;
    public Vector3 position;

    public EnemyPoint(Vector3 pos, EnemyTypes type) {
        position = pos;
        requestEnemy = type;
    }
}

[System.Serializable]
public class MovingPlatformPoint
{
    public Sprite sprite;
    public Vector3 position;
    public Vector3[] localWayPoints;
    public float moveSpeed;
    public bool cyclic;
    public float waitTime;
    public float easeAmount;
    public MovingPlatformPoint(Vector3 pos, Sprite spr, Vector3[] points, float speed, bool cycle, float time, float ease) {
        position = pos;
        sprite = spr;
        localWayPoints = points;
        moveSpeed = speed;
        cyclic = cycle;
        waitTime = time;
        easeAmount = ease;
    }
}


[System.Serializable]
public class PlayerPoint {
    public int _index = 0;
    public int _targetRoomNumber = 0;
    public int _targetIndex = 0;
    public Vector2 _spawnPos;
    public Vector2 _size;
    public Vector2 _offset;
    public Vector2 _position;
    public int _playerMask;

    public PlayerPoint(Vector2 pos, Vector2 size, Vector2 offset, Vector2 spawnPos, int index, int targetRoom, int targetIndex) {
        _position = pos;
        _size = size;
        _offset = offset;
        _spawnPos = spawnPos;
        _index = index;
        _targetRoomNumber = targetRoom;
        _targetIndex = targetIndex;
        _playerMask = 1 << LayerMask.NameToLayer("Player");
    } 

    public Vector3 SpawnPos { 
        get {
            Vector3 pos = _position + _spawnPos; 
            return pos;
        }
    }

    public int Index { get { return _index; } }

    public void FixedProgress() {
        Vector2 position = _position + _offset;
        
        var collider = Physics2D.OverlapBox(position, _size, 0f, _playerMask);
        if (collider) {
            OnCollisionWithPlayer(collider.transform.position);
        }
    }

    private void OnCollisionWithPlayer(Vector3 playerPos) {
        Vector3 diff = playerPos - (Vector3)_position;
        diff.x = 0f;
        RoomManager.GetInstance().MoveRoom(diff, _targetRoomNumber, _targetIndex);
    }
}