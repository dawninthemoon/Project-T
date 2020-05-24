using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomInfo
{
    private RoomBase _roomBase;
    public int RoomNumber { get { return _roomBase.roomNumber; } }

    public RoomInfo(RoomBase roomBase) {
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

        tilemap.ClearAllTiles();
        tilemap.SetTiles(_roomBase.collisionPair.positions, _roomBase.collisionPair.tileBases);
    }

    public void ResetRoom() {
        ObjectManager.GetInstance().ReturnAllEnemies();
    }

    public void FixedProgress() {
        for (int i=0; i < _roomBase.playerPoints.Length; i++) {
            _roomBase.playerPoints[i].FixedProgress();
        }
    }

    public Vector3 GetDoorPosition(int index) {
        if (index >= _roomBase.playerPoints.Length) {
            Debug.LogError("Door does not exists");
            return Vector3.zero;
        }
        return _roomBase.playerPoints[index].SpawnPos;
    }
}
