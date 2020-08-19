using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TilemapPair {
    public Vector3Int[] positions;
    public TileBase[] tileBases;

    public TilemapPair(Vector3Int[] pos, TileBase[] tile) {
        positions = pos;
        tileBases = tile;
    } 
}

[System.Serializable]
public class SORoomBase : ScriptableObject
{
    public int roomNumber;
    public TilemapPair collisionPair;
    public TilemapPair throughPair;
    public PlayerPoint[] playerPoints;
    public EnemyPoint[] enemyPoints;
    public WaterPoint[] waterPoints;
    public PolygonColliderPoint colliderPoint;
    public MovingPlatformPoint[] movingPlatformPoints;
}
