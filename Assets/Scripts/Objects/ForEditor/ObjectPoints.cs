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
    public int index = 0;
    public int targetRoomNumber = 0;
    public int targetIndex = 0;
    public bool vertical;
    public Vector2 size;
    public Vector2 offset;
    public Vector2 position;
    public int playerMask;

    public PlayerPoint(Vector2 pos, Vector2 size, Vector2 offset, bool vertical, int index, int targetRoom, int targetIndex) {
        position = pos;
        this.size = size;
        this.offset = offset;
        this.vertical = vertical;
        this.index = index;
        targetRoomNumber = targetRoom;
        this.targetIndex = targetIndex;
        playerMask = 1 << LayerMask.NameToLayer("Player");
    }

    public void FixedProgress() {
        if (targetRoomNumber == -1) return;

        Vector2 position = this.position + offset;
        
        var collider = Physics2D.OverlapBox(position, size, 0f, playerMask);
        if (collider) {
            OnCollisionWithPlayer(collider.transform.position);
        }
    }

    private void OnCollisionWithPlayer(Vector3 playerPos) {
        Vector3 diff = playerPos - (Vector3)position;
        diff.x = 0f;
        RoomManager.GetInstance().MoveRoom(diff, targetRoomNumber, targetIndex);
    }
}

[System.Serializable]
public class WaterPoint {
    public Vector3 position;
    public Vector3 scale;
}

[System.Serializable]
public class PolygonColliderPoint {
    public Vector2[] points;
}