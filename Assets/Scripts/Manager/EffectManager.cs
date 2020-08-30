using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SingletonWithMonoBehaviour<EffectManager>
{
    private ObjectPool<EffectBase> _effectPool;
    private AssetLoader _assetLoader;
    private List<EffectBase> _acitveEffects;
    private CameraShake _cameraShake;

    public void Initialize() {
        _assetLoader = AssetLoader.GetInstance();
        _effectPool = new ObjectPool<EffectBase>(20, CreateEffect);
        _acitveEffects = new List<EffectBase>();
        _cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    public void Progress() {
        for (int i = 0; i < _acitveEffects.Count; i++) {
            if (_acitveEffects[i].OnEffectUpdate()) {
                _acitveEffects.RemoveAt(i--);
            }
        }
    }

    public void ShakeCamera(float duration) {
        _cameraShake.StartShakeCamera(duration);
    }

    private EffectBase CreateEffect() {
        EffectBase effect = new GameObject("Effect").AddComponent<EffectBase>();
        effect.Initalize();
        return effect;
    }

    public void SpawnAndRemove(Vector3 pos, string effectName, float dir = 1f) {
        EffectBase effect = _effectPool.GetObject();

        SpriteAtlasAnimator.OnAnimationEnd callback = () => { _effectPool.ReturnObject(effect);};
        effect.SetEffectInfo(pos, effectName, dir, callback);

        _acitveEffects.Add(effect);
    }

    public void SpawnTrackEffectAndRemove(Vector3 pos, string effectName, Transform target, float dir = 1f) {
        EffectBase effect = _effectPool.GetObject();
        System.Action onEffectUpdate = () => { effect.transform.position = target.position; };
        SpriteAtlasAnimator.OnAnimationEnd callback = () => { _effectPool.ReturnObject(effect);};
        effect.SetEffectInfo(pos, effectName, dir, callback, onEffectUpdate);
    
        _acitveEffects.Add(effect);
    }

    public void SpawnEffect(Vector3 pos, string effectName, SpriteAtlasAnimator.OnAnimationEnd onEffectEnd, float dir = 1f) {
        EffectBase effect = _effectPool.GetObject();
        RuntimeAnimatorController controller = _assetLoader.GetAnimatorController(effectName);
        effect.SetEffectInfo(pos, effectName, dir, onEffectEnd);

        _acitveEffects.Add(effect);
    }
}
