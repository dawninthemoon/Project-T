using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Soul : MonoBehaviour
{
    private ObjectPool<Soul> _soulPool;
    private Transform _playerTransform;
    [SerializeField] private Vector3[] _soulSize = null;
    [SerializeField] private int[] _fragmentCount = null;
    private int _soulIndex;
    public bool Simulated { get; private set; } = false;

    public void Initalize(ObjectPool<Soul> pool, Transform player, int index) {
        _soulPool = pool;
        _playerTransform = player;
        _soulIndex = index;
        transform.localScale = _soulSize[index];

        Invoke("SetSimulate", 0.5f);
    }

    public void OnHit() {
        float dir = Mathf.Sign(_playerTransform.localScale.x);
        var prefab = AssetLoader.GetInstance().GetPrefab("SoulFragment");

        for (int i = 0; i < _fragmentCount[_soulIndex]; ++i) {
            var fragment = Instantiate(prefab).GetComponent<SoulFragment>();

            float radian = Random.Range(-Mathf.PI / 4f, Mathf.PI / 4f);
            Vector3 direction = new Vector3(Mathf.Cos(radian) * dir, Mathf.Sin(radian)).normalized;

            fragment.Spawn(transform.position, direction * Random.Range(1.5f, 1.8f), _playerTransform);
        }
        RemoveSelf();
    }

    private void SetSimulate() {
        Simulated = true;
    }

    public void RemoveSelf() {
        Simulated = false;
        _soulPool.ReturnObject(this);
    }
}
