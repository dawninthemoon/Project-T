﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class EffectBase : MonoBehaviour
{
    private System.Action _onEffectUpdateCallback;
    private SpriteAtlasAnimator _animator;
    private SpriteRenderer _renderer;
    private bool _isAnimationEnd;
    private float _lifeTime;
    public float Speed { get; set; }

    public void Initalize() {
        _renderer = gameObject.AddComponent<SpriteRenderer>();
        _animator = new SpriteAtlasAnimator();
        _renderer.sortingLayerName = "Effect";
    }

    public void Progress(SpriteAtlas atlas) {
        _animator.Progress(_renderer, atlas);
    }

    public bool OnEffectUpdate() {
        _onEffectUpdateCallback?.Invoke();
        return _isAnimationEnd;
    }

    public void SetEffectInfo(Vector3 pos, string prefix, float dir, SpriteAtlasAnimator.OnAnimationEnd endCallback, System.Action updateCallback = null) {
        _isAnimationEnd = false;
        transform.position = pos;
        transform.localScale = Aroma.VectorUtility.GetScaleVec(dir);

        _animator.Initalize(prefix, "");
        endCallback += () => _isAnimationEnd = true;

        _animator.ChangeAnimation("", false, endCallback);
        _onEffectUpdateCallback = updateCallback;
    }

    public void SetEffectInfoWithCondition(Vector3 pos, string prefix, float dir, bool loop, System.Action updateCallback,  EffectManager.IsEffectEnd isEndCallback, SpriteAtlasAnimator.OnAnimationEnd endCallback) {
        _isAnimationEnd = false;
        transform.position = pos;

        _animator.Initalize(prefix, "", loop);

        updateCallback += () => {
            if (isEndCallback()) {
                _isAnimationEnd = true;
                endCallback();
            }
        };

        _animator.ChangeAnimation("", loop, null);
        _onEffectUpdateCallback = updateCallback;
    }

    public void SetEffectInfoWithDuration(Vector3 pos, string prefix, float dir, float duration, bool loop, SpriteAtlasAnimator.OnAnimationEnd endCallback) {
        _lifeTime = duration;

        EffectManager.IsEffectEnd isEndCallback = () => {
            _lifeTime -= Time.deltaTime;
            if (IsLifetimeEnd()) {
                return true;
            }
            return false;
        };

        SetEffectInfoWithCondition(pos, prefix, dir, loop, null, isEndCallback, endCallback);
    }

    public void SetEffectInfoWithDuration(Vector3 pos, string prefix, float dir, float duration, bool loop, System.Action updateCallback, SpriteAtlasAnimator.OnAnimationEnd endCallback) {
        _lifeTime = duration;

        EffectManager.IsEffectEnd isEndCallback = () => {
            _lifeTime -= Time.deltaTime;
            if (IsLifetimeEnd()) {
                return true;
            }
            return false;
        };

        SetEffectInfoWithCondition(pos, prefix, dir, loop, updateCallback, isEndCallback, endCallback);
    }

    public bool IsLifetimeEnd() {
        return _lifeTime < 0f;
    }
}
