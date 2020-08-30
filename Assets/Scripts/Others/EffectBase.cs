using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class EffectBase : MonoBehaviour
{
    private System.Action _onEffectUpdateCallback;
    private SpriteAtlasAnimator _animator;
    private SpriteRenderer _renderer;
    private bool _isAnimationEnd;

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
}
