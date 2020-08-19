using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasAnimator : MonoBehaviour
{
    [SerializeField] private SpriteAtlas _spriteAtlas;
    private string _base = "PLAYER.Sprites._";
    private int _num = 0;
    private static readonly float Delay = 1f / 12f;
    private float _indexTimer = Delay;

    private void Update() {
        _indexTimer -= Time.deltaTime;
        if (_indexTimer < 0f) {
            _indexTimer = Delay;
            var sprite = _spriteAtlas.GetSprite(_base + _num.ToString());
            GetComponent<SpriteRenderer>().sprite = sprite;
            _num = (_num + 1) % 12;
        }
    }
}
