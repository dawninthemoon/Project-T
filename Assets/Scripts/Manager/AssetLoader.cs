using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Aroma;

public class AssetLoader : SingletonWithMonoBehaviour<AssetLoader>
{
    private AssetBundle _assetBundle;

    private static readonly string AssetBundlePath = "/AssetBundles";
    private static readonly string AssetBundleName = "assetbundle_0";

    public void Initalize() {
        _assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath + AssetBundlePath, AssetBundleName));

        if (_assetBundle == null) {
            Debug.LogError("Failed to load AssetBundle");
            return;
        }
    }

    public RoomBase[] GetAllRoomBases() {
        if (_assetBundle == null)
            Initalize();

        RoomBase[] wholeRooms = _assetBundle.LoadAllAssets<RoomBase>();
        return wholeRooms;
    }

    public GameObject GetPrefab(string prefabName) {
        GameObject prefab = _assetBundle.LoadAsset<GameObject>(prefabName);
        return prefab;
    }

    public EnemyBase[] GetAllEnemies() {
        GameObject[] prefabs = _assetBundle.LoadAllAssets<GameObject>();
        List<EnemyBase> enemies = new List<EnemyBase>();

        for (int i = 0; i < prefabs.Length; i++) {
            var enemy = prefabs[i].GetComponentNoAlloc<EnemyBase>();
            if (enemy != null) {
                enemies.Add(enemy);
            }
        }

        return enemies.ToArray();
    }

    public RuntimeAnimatorController GetAnimatorController(string fileName) {
        var controller = _assetBundle.LoadAsset<RuntimeAnimatorController>(fileName);
        return controller;
    }
}
