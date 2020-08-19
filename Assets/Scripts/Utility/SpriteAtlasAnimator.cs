using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasAnimator : MonoBehaviour
{
    [SerializeField] private SpriteAtlas _spriteAtlas;
    private SpriteRenderer _renderer;
    private static readonly string Sufix = ".Sprites._";
    private static readonly float _defaultSpeed = 0.06f;
    private string _prefix;
    private float _indexTimer = 0f;
    private int _spriteIndex;
    private string _animationName;
    private bool _loop;

    public virtual void Initalize(string prefix, string idleStateName, bool loop = false) {
        _prefix = prefix;
        _renderer = GetComponent<SpriteRenderer>();
        ChangeAnimation(idleStateName, loop);
    }

    public void ChangeAnimation(string name, bool loop = false) {
        _animationName = name;
        _loop = loop;   
    }

    public void Progress() {
        _indexTimer += Time.deltaTime;
        if (_indexTimer > _defaultSpeed) {
            _indexTimer = 0f;

            var currentFrame = GetSprite();
            if (currentFrame == null) {
                _spriteIndex = 0;
                currentFrame = GetSprite();
            }

            _renderer.sprite = currentFrame;
            ++_spriteIndex;
        }
    }

    Sprite GetSprite() {
        string spriteName = Aroma.StringUtils.MergeStrings(_prefix, _animationName, Sufix, _spriteIndex.ToString());
        var currentFrame = _spriteAtlas.GetSprite(spriteName);
        return currentFrame;
    }
}
