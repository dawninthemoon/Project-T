using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct TilemapPair {
    Vector3Int[] positions;
    TileBase[] tileBases;

    public TilemapPair(Vector3Int[] pos, TileBase[] tile) {
        positions = pos;
        tileBases = tile;
    } 
}

public class RoomBase : ScriptableObject
{
    public int roomNumber;
    public TilemapPair tilemapPair;
    public PlayerSpawnPosition[] playerSpawnPositions;
    public EnemySpawnPosition[] enemySpawnPositions;
}
