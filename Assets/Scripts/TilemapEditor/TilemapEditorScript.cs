﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TilemapEditorScript : MonoBehaviour
{
    public EnemySpawnPosition enemyPointPrefab = null;
    public PlayerSpawnPosition playerPointPrefab = null;
    public PlatformController movingPlatformPrefab = null;

    public void ClearAll() {
        ClearAllTilemaps();
        ClearAllObjects();
    }

    public void ClearAllTilemaps() {
        Tilemap collisionTilemap = transform.Find("Collision").GetComponent<Tilemap>();
        Tilemap throughTilemap = transform.Find("Through").GetComponent<Tilemap>();

        collisionTilemap.ClearAllTiles();
        throughTilemap.ClearAllTiles();
    }

    public void ClearAllObjects() {
        Transform objectTilemap = transform.Find("Objects");
        var tempArray = new GameObject[objectTilemap.childCount];

        for (int i = 0; i < objectTilemap.childCount; i++) {
            tempArray[i] = objectTilemap.GetChild(i).gameObject;
        }

        foreach(var child in tempArray)
        {
            DestroyImmediate(child);
        }
    }

    public RoomBase RequestExport() {
        Tilemap collisionTilemap = transform.Find("Collision").GetComponent<Tilemap>();
        Tilemap throughTilemap = transform.Find("Through").GetComponent<Tilemap>();
        Transform objectTilemap = transform.Find("Objects");

        var enemies = objectTilemap.GetComponentsInChildren<EnemySpawnPosition>(true);
        var playerPoints =  objectTilemap.GetComponentsInChildren<PlayerSpawnPosition>(true);
        var movingPlatforms = objectTilemap.GetComponentsInChildren<PlatformController>(true);

        RoomBase asset = ScriptableObject.CreateInstance<RoomBase>();

        asset.collisionPair = LoadTileInfo(collisionTilemap);
        asset.throughPair = LoadTileInfo(throughTilemap);
        asset.enemyPoints = LoadEnemyPoints(enemies);
        asset.playerPoints = LoadPlayerPoints(playerPoints);
        asset.movingPlatformPoints = LoadMovingPlatformPoints(movingPlatforms);

        return asset;
    }

    private TilemapPair LoadTileInfo(Tilemap tilemap) {
        BoundsInt bounds = tilemap.cellBounds;
        List<Vector3Int> positions = new List<Vector3Int>();
        List<TileBase> tileBases = new List<TileBase>();

        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin) {
            if (!tilemap.HasTile(pos)) continue;
            positions.Add(pos);
            tileBases.Add(tilemap.GetTile(pos));
        }

        return new TilemapPair(positions.ToArray(), tileBases.ToArray());
    }

    public EnemyPoint[] LoadEnemyPoints(EnemySpawnPosition[] info) {
        EnemyPoint[] enemyPoint = new EnemyPoint[info.Length];
        for (int i = 0; i < info.Length; i++) {
            enemyPoint[i] = new EnemyPoint(info[i].transform.position, info[i].RequestEnemy);
        }
        return enemyPoint;
    }

    public PlayerPoint[] LoadPlayerPoints(PlayerSpawnPosition[] info) {
        PlayerPoint[] points = new PlayerPoint[info.Length];
        for (int i = 0; i < info.Length; i++) {
            var collider = info[i].GetComponent<BoxCollider2D>();

            points[i] = new PlayerPoint(
                info[i].transform.position,
                collider.size,
                collider.offset,
                info[i]._spawnPos,
                info[i]._index,
                info[i]._targetRoomNumber,
                info[i]._targetIndex
            );
        }

        return points;
    }

    public MovingPlatformPoint[] LoadMovingPlatformPoints(PlatformController[] info) {
        MovingPlatformPoint[] points = new MovingPlatformPoint[info.Length];
        for (int i = 0; i < info.Length; i++) {
            Sprite spr = info[i].GetComponent<SpriteRenderer>().sprite;
            points[i] = new MovingPlatformPoint(
                info[i].transform.position,
                spr,
                info[i].LocalWayPoints,
                info[i].MoveSpeed,
                info[i].Cyclic,
                info[i].WaitTime,
                info[i].EaseAmount
            );
        }
        return points;
    }

    public void Import(RoomBase roomBase) {
        ClearAll();

        Tilemap collisionTilemap = transform.Find("Collision").GetComponent<Tilemap>();
        Tilemap throughTilemap = transform.Find("Through").GetComponent<Tilemap>();
        Transform objectTilemap = transform.Find("Objects");

        collisionTilemap.SetTiles(roomBase.collisionPair.positions, roomBase.collisionPair.tileBases);
        throughTilemap.SetTiles(roomBase.throughPair.positions, roomBase.throughPair.tileBases);

        foreach (var enemy in roomBase.enemyPoints) {
            var obj = Instantiate(enemyPointPrefab, objectTilemap);
            obj.transform.position = enemy.position;
            obj._requestEnemy = enemy.requestEnemy;
        }
        
        foreach (var point in roomBase.playerPoints) {
            var obj = Instantiate(playerPointPrefab, objectTilemap);
            obj.transform.position = point._position;
            obj._index = point.Index;
            obj._targetIndex = point._targetIndex;
            obj._targetRoomNumber = point._targetRoomNumber;
            obj._spawnPos = point._spawnPos;
            obj._collider = obj.GetComponent<BoxCollider2D>();
        }

        foreach (var point in roomBase.movingPlatformPoints) {
            var obj = Instantiate(movingPlatformPrefab, objectTilemap);
            obj.Initialize();
            obj.Setup(point);
        }
    }
}
