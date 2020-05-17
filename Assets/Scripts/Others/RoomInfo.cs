using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [SerializeField] private Transform _objectTilemap = null;
    [SerializeField] private int _roomNumber = 0;
    public int RoomNumber { get { return _roomNumber; } }
    
    private EnemySpawnPosition[] _enemies = null;
    private PlayerSpawnPosition[] _doors = null;

    public void Initalize() {
        _enemies = _objectTilemap.GetComponentsInChildren<EnemySpawnPosition>();
        _doors = _objectTilemap.GetComponentsInChildren<PlayerSpawnPosition>();
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
    }

    public void StartRoom() {
        var objManager = ObjectManager.GetInstance();
        gameObject.SetActive(true);

        for (int i = 0; i < _enemies.Length; i++) {
            EnemyTypes type = _enemies[i].RequestEnemy;
            Vector3 position = _enemies[i].transform.position;

            objManager.SpawnEnemy(type, position);
        }
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
