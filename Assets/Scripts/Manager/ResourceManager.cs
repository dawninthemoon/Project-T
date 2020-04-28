using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private static readonly Type GameObjectType = typeof(GameObject);
    //private static readonly Type SpritetType = typeof(Sprite);
    private static readonly string prefabFilePath = "Prefabs/";

    public GameObject GetPrefab(string fileName)
	{
		string path = prefabFilePath + fileName;
		GameObject obj = Load(path,GameObjectType) as GameObject;

		return obj;
	}
/*
    public Sprite GetSprite(string fileName)
	{
		if(sprite.ContainsKey(fileName))
			return sprite[fileName];

		string path = spritesFilePath + fileName;
		
		if(Load(path, spriteType) != null)
		{
			if(Load(path, spriteType) as Sprite == null)
				Debug.Log("what the fuck");
		}

		Sprite obj = Load(path,spriteType) as Sprite;
		if(obj == null)
		{
			Debug.Log("file does not exist : " + path);
			return null;
		}
		sprite.Add(fileName,obj);

		return obj;
	}
*/

    public UnityEngine.Object Load(string path, System.Type type)
	{
		return Resources.Load(path, type);
	}

	public UnityEngine.Object[] LoadAll(string path, System.Type type)
	{
		return Resources.LoadAll(path,type);
	}
}
