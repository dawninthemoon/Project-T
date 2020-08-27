using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelEditor {
    [ExecuteInEditMode]
    public class TilemapEditorScript : MonoBehaviour
    {
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

        public SORoomBase RequestExport() {
            Tilemap collisionTilemap = transform.Find("Collision").GetComponent<Tilemap>();
            Tilemap throughTilemap = transform.Find("Through").GetComponent<Tilemap>();
            Transform objectTilemap = transform.Find("Objects");

            var enemies = objectTilemap.GetComponentsInChildren<EnemySpawnPosition>(true);
            var playerPoints =  objectTilemap.GetComponentsInChildren<PlayerSpawnPosition>(true);
            var movingPlatforms = objectTilemap.GetComponentsInChildren<PlatformController>(true);
            var water = objectTilemap.GetComponentsInChildren<Water>(true);
            var collider = objectTilemap.GetComponentInChildren<PolygonCollider2D>(true);

            SORoomBase asset = ScriptableObject.CreateInstance<SORoomBase>();

            asset.collisionPair = LoadTileInfo(collisionTilemap);
            asset.throughPair = LoadTileInfo(throughTilemap);
            asset.enemyPoints = LoadEnemyPoints(enemies);
            asset.playerPoints = LoadPlayerPoints(playerPoints);
            asset.movingPlatformPoints = LoadMovingPlatformPoints(movingPlatforms);
            asset.waterPoints = LoadWaterPoints(water);
            asset.colliderPoint = LoadColliderPoint(collider);

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

        private PolygonColliderPoint LoadColliderPoint(PolygonCollider2D info) {
            if (info == null) {
                Debug.LogError("Polygon Collider has not found.");
            }

            int pointsCount = info.GetPath(0).Length;
            Vector2[] pointsArr = new Vector2[pointsCount];
            for (int i=0; i < pointsCount; ++i) {
                pointsArr[i] = info.GetPath(0)[i];
            }

            var point = new PolygonColliderPoint {
                points = pointsArr
            };

            return point;
        }

        public WaterPoint[] LoadWaterPoints(Water[] info) {
            WaterPoint[] waterPoints = new WaterPoint[info.Length];
            for (int i=0; i<info.Length; ++i) {
                waterPoints[i] = new WaterPoint() {
                    position = info[i].transform.position,
                    scale = info[i].transform.localScale
                };
            }
            return waterPoints;
        }

        public EnemyPoint[] LoadEnemyPoints(EnemySpawnPosition[] info) {
            EnemyPoint[] enemyPoint = new EnemyPoint[info.Length];
            for (int i = 0; i < info.Length; ++i) {
                enemyPoint[i] = new EnemyPoint(info[i].transform.position, info[i].requestEnemy);
            }
            return enemyPoint;
        }

        public PlayerPoint[] LoadPlayerPoints(PlayerSpawnPosition[] info) {
            PlayerPoint[] points = new PlayerPoint[info.Length];
            for (int i = 0; i < info.Length; i++) {
                var collider = info[i].GetComponent<BoxCollider2D>();

                points[i] = new PlayerPoint(
                    info[i].transform.position,
                    info[i].GetColliderSize(),
                    info[i].GetColliderOffset(),
                    info[i].vertical,
                    info[i].index,
                    info[i].targetRoomNumber,
                    info[i].targetIndex
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

        public void Import(SORoomBase roomBase, EnemySpawnPosition enemyPointPrefab, PlayerSpawnPosition playerPointPrefab, PlatformController movingPlatformPrefab, Water waterPrefab) {
            ClearAll();

            Tilemap collisionTilemap = transform.Find("Collision").GetComponent<Tilemap>();
            Tilemap throughTilemap = transform.Find("Through").GetComponent<Tilemap>();
            Transform objectTilemap = transform.Find("Objects");

            collisionTilemap.SetTiles(roomBase.collisionPair.positions, roomBase.collisionPair.tileBases);
            throughTilemap.SetTiles(roomBase.throughPair.positions, roomBase.throughPair.tileBases);

            foreach (var enemy in roomBase.enemyPoints) {
                var obj = Instantiate(enemyPointPrefab, objectTilemap);
                obj.transform.position = enemy.position;
                obj.requestEnemy = enemy.requestEnemy;
            }
            
            foreach (var point in roomBase.playerPoints) {
                var obj = Instantiate(playerPointPrefab, objectTilemap);
                obj.transform.position = point.position;
                obj.index = point.index;
                obj.targetIndex = point.targetIndex;
                obj.targetRoomNumber = point.targetRoomNumber;
                obj.vertical = point.vertical;
                obj.transform.localScale = new Vector3(Mathf.Round(point.size.x), Mathf.Round(point.size.y), 1f);
            }

            foreach (var point in roomBase.movingPlatformPoints) {
                var obj = Instantiate(movingPlatformPrefab, objectTilemap);
                obj.Initialize();
                obj.Setup(point);
            }

            foreach (var point in roomBase.waterPoints) {
                var obj = Instantiate(waterPrefab, objectTilemap);
                obj.transform.localScale = point.scale;
                obj.transform.position = point.position;
            }

            var collider = new GameObject().AddComponent<PolygonCollider2D>();
            collider.SetPath(0, roomBase.colliderPoint.points);
            collider.transform.SetParent(objectTilemap);
        }
    }
}