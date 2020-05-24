using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TilemapEditorScript : MonoBehaviour
{
    public RoomBase RequestExport() {
        Tilemap collisionTilemap = transform.Find("Collision").GetComponent<Tilemap>();
        Tilemap throughTilemap = transform.Find("Through").GetComponent<Tilemap>();;
        Transform objectTilemap = transform.Find("Objects");

        var enemies = objectTilemap.GetComponentsInChildren<EnemySpawnPosition>(true);
        var playerPoints =  objectTilemap.GetComponentsInChildren<PlayerSpawnPosition>(true);

        RoomBase asset = ScriptableObject.CreateInstance<RoomBase>();

        asset.collisionPair = LoadTileInfo(collisionTilemap);
        asset.throughPair = LoadTileInfo(throughTilemap);
        asset.enemyPoints = LoadEnemyPoints(enemies);
        asset.playerPoints = LoadPlayerPoints(playerPoints);

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
        for (int i=0; i < info.Length; i++) {
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
}
