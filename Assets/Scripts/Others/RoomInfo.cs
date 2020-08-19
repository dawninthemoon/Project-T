using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomInfo
{
    private SORoomBase _roomBase;
    public int RoomNumber { get { return _roomBase.roomNumber; } }

    public RoomInfo(SORoomBase roomBase) {
        _roomBase = roomBase;
    }

    public void StartRoom(Tilemap tilemap) {
        var objManager = ObjectManager.GetInstance();

        var enemies = _roomBase.enemyPoints;
        for (int i = 0; i < enemies.Length; i++) {
            EnemyTypes type = enemies[i].requestEnemy;
            Vector3 position = enemies[i].position;

            objManager.SpawnEnemy(type, position);
        }

        var movingPlatforms = _roomBase.movingPlatformPoints;
        for (int i = 0; i < movingPlatforms.Length; i++) {
            objManager.CreateMovingPlatform(movingPlatforms[i]);
        }

        var waterPoints = _roomBase.waterPoints;
        foreach (var waterObj in waterPoints) {
            objManager.CreateWater(waterObj);
        }

        tilemap.ClearAllTiles();
        tilemap.SetTiles(_roomBase.collisionPair.positions, _roomBase.collisionPair.tileBases);
    }

    public void ResetRoom() {
        ObjectManager.GetInstance().ReturnAllEnemies();
        ObjectManager.GetInstance().ReturnAllPlatforms();
        ObjectManager.GetInstance().ReturnAllWater();
    }

    public void FixedProgress() {
        for (int i=0; i < _roomBase.playerPoints.Length; i++) {
            _roomBase.playerPoints[i].FixedProgress();
        }
    }

    public bool IsDoorVertical(int index) {
        if (index >= _roomBase.playerPoints.Length) {
            Debug.LogError("Door does not exists");
            return false;
        }
        return _roomBase.playerPoints[index].vertical;
    }

    public Vector3 GetDoorPosition(int index) {
        if (index >= _roomBase.playerPoints.Length) {
            Debug.LogError("Door does not exists");
            return Vector3.zero;
        }
        return _roomBase.playerPoints[index].position;
    }

    public Vector2[] GetColliderPath(int index) {
        return _roomBase.colliderPoint.points;
    }
}
