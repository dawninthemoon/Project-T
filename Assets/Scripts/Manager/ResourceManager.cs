using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private static readonly Type GameObjectType = typeof(GameObject);
    private static readonly Type AnimatorControllerType = typeof(RuntimeAnimatorController);
    //private static readonly Type SpritetType = typeof(Sprite);
    private static readonly string PrefabFilePath = "Prefabs/";
    private static readonly string AnimatorControllerFilePath = "AnimatorControllers/";

    private Dictionary<string, RuntimeAnimatorController> _animatorControllers;

    public void Initialize() {
        _animatorControllers = new Dictionary<string, RuntimeAnimatorController>();
    }

    public GameObject GetPrefab(string fileName)
	{
		string path = PrefabFilePath + fileName;
		GameObject obj = Load(path, GameObjectType) as GameObject;
		return obj;
	}

    public GameObject[] GetAllPrefabs(string directoryName) {
        string path = PrefabFilePath + directoryName;
        UnityEngine.Object[] objects = LoadAll(path, GameObjectType);

        if (objects.Length == 0) {
            Debug.LogError("Prefab does not exists");
            return null;
        }

        GameObject[] gameObjects = new GameObject[objects.Length];
        for (int i = 0; i < objects.Length; i++) {
            gameObjects[i] = objects[i] as GameObject;
        }
        
        return gameObjects;
    }
    
    public RuntimeAnimatorController GetAnimatorController(string fileName) {
        if (_animatorControllers.ContainsKey(fileName))
            return _animatorControllers[fileName];

        string path = AnimatorControllerFilePath + fileName;

        RuntimeAnimatorController resource = Load(path, AnimatorControllerType) as RuntimeAnimatorController;
        if (resource == null) {
            Debug.LogError("The file does not exist: " + path);
            return null;
        }

        _animatorControllers.Add(fileName, resource);
        return resource;
    }

    public UnityEngine.Object Load(string path, System.Type type)
	{
		return Resources.Load(path, type);
	}

	public UnityEngine.Object[] LoadAll(string path, System.Type type)
	{
		return Resources.LoadAll(path, type);
	}
}
