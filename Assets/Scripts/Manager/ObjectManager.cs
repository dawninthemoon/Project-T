using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : SingletonWithMonoBehaviour<ObjectManager>
{
    private ResourceManager _resourceManager;

    private Character _player;
    private List<EnemyBase> _enemyList;

    public void Initialize() {
        _resourceManager = ResourceManager.GetInstance();
        _enemyList = new List<EnemyBase>();

        _resourceManager.Initialize();
    }

    public Character CreateCharacter(Vector3 pos) {
        if (_player == null) {
            var prefab = _resourceManager.GetPrefab("Character");
            _player = Instantiate(prefab, pos, Quaternion.identity).GetComponent<Character>();
        }
        return _player;
    }
}
