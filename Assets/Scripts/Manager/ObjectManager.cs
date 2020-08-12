using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

public class ObjectManager : SingletonWithMonoBehaviour<ObjectManager>
{
    private static readonly string EnemyPrefabDirectory = "Enemy/";
    private static readonly string MovingPlatformName = "MovingPlatform";
    private static readonly string WaterName = "Water";

    private AssetLoader _assetLoader;

    private Player _player;
    private List<EnemyBase> _activeEnemies;
    private List<PlatformController> _activePlatforms;
    private List<Water> _activeWater;
    private ObjectPool<EnemyBase>[] _enemyObjectPoolArr;
    private ObjectPool<PlatformController> _movingPlatformPool;
    private ObjectPool<Water> _waterPool;
    private Dictionary<EnemyTypes, int>  _enemyObjectPoolOrder;

    public void Initialize() {
        _assetLoader = AssetLoader.GetInstance();
        _activeEnemies = new List<EnemyBase>(5);
        _activePlatforms = new List<PlatformController>(5);
        _activeWater = new List<Water>(3);
        _enemyObjectPoolOrder = new Dictionary<EnemyTypes, int>();
        InitalizeObjectPool();
    }

    public void FixedProgress() {
        for (int i = 0; i < _activeEnemies.Count; i++) {
            _activeEnemies[i].FixedProgress();
        }

        for (int i = 0; i < _activePlatforms.Count; i++) {
            _activePlatforms[i].FixedProgress();
        }
    }

    public Player CreatePlayer(Vector3 position) {
        if (_player == null) {
            var prefab = _assetLoader.GetPrefab("Player");
            _player = Instantiate(prefab, position, Quaternion.identity).GetComponent<Player>();
        }
        return _player;
    }

    public void SetPlayerPos(Vector3 position, bool vertical = false) {
        Vector3 extraPos = _player.Velocity;
        if (vertical) {
            extraPos.x = 0f;
            extraPos.y = Mathf.Sign(extraPos.y);
        }
        else {
            extraPos.y = 0f;
            extraPos.x = Mathf.Sign(extraPos.x);
        }
        _player.transform.position = position + extraPos;
    }

    public void ReturnAllEnemies() {
        for (int i = 0; i < _activeEnemies.Count; i++) {
            EnemyTypes type = EnemyUtility.ObjToEnemy(_activeEnemies[i].gameObject);
            int index = _enemyObjectPoolOrder[type];

            _enemyObjectPoolArr[index].ReturnObject(_activeEnemies[i]);
        }

        _activeEnemies.Clear();
    }

    public void ReturnAllPlatforms() {
        for (int i = 0; i < _activePlatforms.Count; i++) {
            _movingPlatformPool.ReturnObject(_activePlatforms[i]);
        }
        _activePlatforms.Clear();
    }

    public void ReturnAllWater() {
        for (int i=0; i < _activeWater.Count; ++i) {
            _waterPool.ReturnObject(_activeWater[i]);
        }
        _activeWater.Clear();
    }

    public void SpawnEnemy(EnemyTypes type, Vector3 position) {
        string fileName = StringUtils.MergeStrings(EnemyPrefabDirectory, type.ToString());
        
        int index = -1;
        _enemyObjectPoolOrder.TryGetValue(type, out index);

        if (index != -1) {
            EnemyBase enemy = _enemyObjectPoolArr[index].GetObject();
            enemy.Reset(position);

            _activeEnemies.Add(enemy);
        }
    }

    public void CreateWater(WaterPoint info) {
        var water = _waterPool.GetObject();
        water.transform.localScale = info.scale;
        water.transform.position = info.position;
        _activeWater.Add(water);
    }

    public void CreateMovingPlatform(MovingPlatformPoint info) {
        var controller = _movingPlatformPool.GetObject();
        controller.Setup(info);
        _activePlatforms.Add(controller);
    }

    private void InitalizeObjectPool() {
        EnemyBase[] enemyPrefabs = _assetLoader.GetAllEnemies();

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

            EnemyTypes type = EnemyUtility.ObjToEnemy(enemyPrefabs[i].gameObject);
            _enemyObjectPoolOrder.Add(type, i);
        }

        _movingPlatformPool = new ObjectPool<PlatformController>(
            5,
            () => {
                GameObject prefab = _assetLoader.GetPrefab(MovingPlatformName);
                PlatformController controller = Instantiate(prefab).GetComponent<PlatformController>();
                controller.Initialize();
                return controller;
            }
        );

        _waterPool = new ObjectPool<Water>(
            5,
            () => {
                GameObject prefab = _assetLoader.GetPrefab(WaterName);
                Water water = Instantiate(prefab).GetComponent<Water>();
                return water;
            }
        );
    }
}
