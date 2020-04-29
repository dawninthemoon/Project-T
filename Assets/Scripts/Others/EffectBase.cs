using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    private System.Action _onEffectEndCallback;
    private Animator _animator;
    private SpriteRenderer _renderer;    

    public void Initalize() {
        _renderer = gameObject.AddComponent<SpriteRenderer>();
        _animator = gameObject.AddComponent<Animator>();
        _renderer.sortingLayerName = "Effect";
    }

    public bool IsEffectEnd() {
        float normalizedTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        return normalizedTime >= 1f;
    }

    public void OnEffectEnd() {
        if (_onEffectEndCallback != null) {
            _onEffectEndCallback.Invoke();
        }
    }

    public void SetEffectInfo(Vector3 pos, RuntimeAnimatorController controller, float dir, System.Action callback) {
        transform.position = pos;
        transform.localScale = Aroma.VectorUtility.GetScaleVec(dir);
        _animator.runtimeAnimatorController = controller;
        _onEffectEndCallback = callback;
    }
}
