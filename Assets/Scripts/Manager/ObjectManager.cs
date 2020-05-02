using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

public class ObjectManager : SingletonWithMonoBehaviour<ObjectManager>
{
    private static readonly string EnemyPrefabDirectory = "Enemy/";

    private ResourceManager _resourceManager;

    private Player _player;
    private List<EnemyBase> _activeEnemies;
    private ObjectPool<EnemyBase>[] _enemyObjectPoolArr;
    private Dictionary<EnemyTypes, int>  _enemyObjectPoolOrder;

    public void Initialize() {
        _resourceManager = ResourceManager.GetInstance();
        _activeEnemies = new List<EnemyBase>();
        _enemyObjectPoolOrder = new Dictionary<EnemyTypes, int>();
        InitalizeObjectPool();
    }

    public void FixedProgress() {
        for (int i = 0; i < _activeEnemies.Count; i++) {
            _activeEnemies[i].FixedProgress();
        }
    }

    public Player CreatePlayer(Vector3 position) {
        if (_player == null) {
            var prefab = _resourceManager.GetPrefab("Player");
            _player = Instantiate(prefab, position, Quaternion.identity).GetComponent<Player>();
        }
        return _player;
    }

    public void SetPlayerPos(Vector3 position) {
        if (position.y == PlayerSpawnPosition.Impossible)
            position.y = _player.transform.position.y;
            
        if (position.x == PlayerSpawnPosition.Impossible)
            position.x = _player.transform.position.x;

        _player.transform.position = position;
    }

    public void ReturnAllEnemies() {
        for (int i = 0; i < _activeEnemies.Count; i++) {
            EnemyTypes type = EnemyUtility.ObjToEnemy(_activeEnemies[i].gameObject);
            int index = _enemyObjectPoolOrder[type];

            _enemyObjectPoolArr[index].ReturnObject(_activeEnemies[i]);
        }

        _activeEnemies.Clear();
    }

    public void SpawnEnemy(EnemyTypes type, Vector3 position) {
        string fileName = EnemyPrefabDirectory + type.ToString();
        
        int index = -1;
        _enemyObjectPoolOrder.TryGetValue(type, out index);

        if (index != -1) {
            EnemyBase enemy = _enemyObjectPoolArr[index].GetObject();
            enemy.Reset(position);

            _activeEnemies.Add(enemy);
        }
    }

    private void InitalizeObjectPool() {
        GameObject[] enemyPrefabs = _resourceManager.GetAllPrefabs(EnemyPrefabDirectory);

        int len = enemyPrefabs.Length;
        _enemyObjectPoolArr = new ObjectPool<EnemyBase>[len];

        for (int i = 0; i < len; i++) {
            _enemyObjectPoolArr[i] = new ObjectPool<EnemyBase>(
                5,
                () => { 
                    EnemyBase enemy = Instantiate(enemyPrefabs[i]).GetComponent<EnemyBase>(); 
                    enemy.name = enemyPrefabs[i].name;
                    enemy.Initialize();
                    return enemy;
                }
            );

            EnemyTypes type = EnemyUtility.ObjToEnemy(enemyPrefabs[i]);
            _enemyObjectPoolOrder.Add(type, i);
        }
    }
}
