using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasAnimator
{
    private static readonly float _defaultSpeed = 0.06f;
    private string _prefix;
    private float _indexTimer = 0f;
    private int _spriteIndex = 1;
    public int SpriteIndex { set { _spriteIndex = SpriteIndex+1; } get { return _spriteIndex-1; } }
    private string _animationName;
    private bool _loop;
    public delegate void OnAnimationEnd();
    private OnAnimationEnd _animationEndCallback;

    public void Initalize(string prefix, string idleStateName, bool loop = false) {
        _prefix = prefix;
        ChangeAnimation(idleStateName, loop);
    }

    public void ChangeAnimation(string name, bool loop = false, OnAnimationEnd callback = null) {
        _indexTimer = 0f;
        _spriteIndex = 1;
        _animationName = name;
        _loop = loop;
        _animationEndCallback = callback;
    }

    public void Progress(SpriteRenderer renderer, SpriteAtlas atlas) {
        _indexTimer += Time.deltaTime;
        if (_indexTimer > _defaultSpeed) {
            _indexTimer = 0f;

            var currentFrame = GetSprite();
            if (currentFrame == null) {
                if (_loop) {
                    _spriteIndex = 1;
                    currentFrame = GetSprite();
                }
                else {
                    _animationEndCallback?.Invoke();
                }
            }

            if (currentFrame != null) {
                renderer.sprite = currentFrame;
                ++_spriteIndex;
            }
        }

        Sprite GetSprite() {
            string spriteName = Aroma.StringUtils.MergeStrings(_prefix, _animationName, _spriteIndex.ToString());
            var currentFrame = atlas.GetSprite(spriteName);
            return currentFrame;
        }
    }
}
