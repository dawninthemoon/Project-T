using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TilemapEditorScript : MonoBehaviour
{
    public void Export() {
        Tilemap collisionTilemap = transform.Find("Collision").GetComponent<Tilemap>();
        Tilemap throughTilemap = transform.Find("Through").GetComponent<Tilemap>();;
        Transform objectTilemaps = transform.Find("Objects");

        TilemapPair collisionTileInfo = LoadTileInfo(collisionTilemap);
        TilemapPair throughTileInfo = LoadTileInfo(throughTilemap);
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
}
