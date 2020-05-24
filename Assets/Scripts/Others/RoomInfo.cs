using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomInfo : MonoBehaviour
{
    [SerializeField] private Transform _objectTilemap = null;
    [SerializeField] private Tilemap _collisionTilemap = null;
    [SerializeField] private Tilemap _throughTilemap = null;
    [SerializeField] private int _roomNumber = 0;
    public int RoomNumber { get { return _roomNumber; } }
    
    private EnemySpawnPosition[] _enemies = null;
    private PlayerSpawnPosition[] _doors = null;
    private KeyValuePair<Vector3Int[], TileBase[]> _collisionTiles;

    public void Initalize() {
        _enemies = _objectTilemap.GetComponentsInChildren<EnemySpawnPosition>(true);
        _doors = _objectTilemap.GetComponentsInChildren<PlayerSpawnPosition>(true);
        System.Array.Sort(
            _doors,
             (PlayerSpawnPosition val1, PlayerSpawnPosition val2) => val1.Index.CompareTo(val2.Index));

        for (int i = 0; i < _enemies.Length; i++) {
            _enemies[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _doors.Length; i++) {
            _doors[i].GetComponent<SpriteRenderer>().enabled = false;
            _doors[i].Initalize();
        }

        LoadAllTiles();
    }

    public void LoadAllTiles() {
        BoundsInt bounds = _collisionTilemap.cellBounds;
        List<Vector3Int> positions = new List<Vector3Int>();
        List<TileBase> tileBases = new List<TileBase>();

        foreach (Vector3Int pos in _collisionTilemap.cellBounds.allPositionsWithin) {
            if (!_collisionTilemap.HasTile(pos)) continue;
            positions.Add(pos);
            tileBases.Add(_collisionTilemap.GetTile(pos));
        }

        _collisionTiles = new KeyValuePair<Vector3Int[], TileBase[]>(
                             positions.ToArray(),
                             tileBases.ToArray());
    }

    public void StartRoom(Tilemap tilemap) {
        var objManager = ObjectManager.GetInstance();
        gameObject.SetActive(true);

        for (int i = 0; i < _enemies.Length; i++) {
            EnemyTypes type = _enemies[i].RequestEnemy;
            Vector3 position = _enemies[i].transform.position;

            objManager.SpawnEnemy(type, position);
        }

        tilemap.ClearAllTiles();
        tilemap.SetTiles(_collisionTiles.Key, _collisionTiles.Value);
    }

    public void ResetRoom() {
        gameObject.SetActive(false);
        ObjectManager.GetInstance().ReturnAllEnemies();
    }

    public void FixedProgress() {
        for (int i=0;i<_doors.Length;i++) {
            _doors[i].FixedProgress();
        }
    }

    public Vector3 GetDoorPosition(int index) {
        if (index >= _doors.Length) {
            Debug.LogError("Door does not exists");
            return Vector3.zero;
        }
        return _doors[index].SpawnPos;
    }
}
