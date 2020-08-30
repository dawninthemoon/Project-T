using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Aroma;
using UnityEngine.U2D;

public class AssetLoader : Singleton<AssetLoader>
{
    private AssetBundle _objectBundle;
    private AssetBundle _roomBundle;

    private static readonly string AssetBundlePath = "/AssetBundles";
    private static readonly string AssetBundleNameBase = "assetbundle_";

    public void Initalize() {
        _objectBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath + AssetBundlePath, AssetBundleNameBase + "object"));
        _roomBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath + AssetBundlePath, AssetBundleNameBase + "room"));

        if (_objectBundle == null || _roomBundle == null) {
            Debug.LogError("Failed to load AssetBundle");
            return;
        }
    }

    public SORoomBase[] GetAllRoomBases() {
        if (_roomBundle == null)
            Initalize();

        SORoomBase[] wholeRooms = _roomBundle.LoadAllAssets<SORoomBase>();
        return wholeRooms;
    }

    public ScriptableObject GetScriptableObject(string name) {
        if (name.Equals("NULL")) return null;
        ScriptableObject scriptableObj = _objectBundle.LoadAsset<ScriptableObject>(name);
        return scriptableObj;
    }  

    public GameObject GetPrefab(string prefabName) {
        GameObject prefab = _objectBundle.LoadAsset<GameObject>(prefabName);
        return prefab;
    }

    public EnemyBase[] GetAllEnemies() {
        GameObject[] prefabs = _objectBundle.LoadAllAssets<GameObject>();
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
        var controller = _objectBundle.LoadAsset<RuntimeAnimatorController>(fileName);
        return controller;
    }
}
