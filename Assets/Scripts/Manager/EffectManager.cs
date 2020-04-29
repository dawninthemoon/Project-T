using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SingletonWithMonoBehaviour<EffectManager>
{
    private ObjectPool<EffectBase> _effectPool;
    private ResourceManager _resourceManager;
    private List<EffectBase> _acitveEffects;

    public void Initialize() {
        _resourceManager = ResourceManager.GetInstance();
        _effectPool = new ObjectPool<EffectBase>(20, CreateEffect);
        _acitveEffects = new List<EffectBase>();
    }

    public void Progress() {
        for (int i = 0; i < _acitveEffects.Count; i++) {
            if (_acitveEffects[i].IsEffectEnd()) {
                _acitveEffects[i].OnEffectEnd();
                _acitveEffects.RemoveAt(i--);
            }
        }
    }

    private EffectBase CreateEffect() {
        EffectBase effect = new GameObject("Effect").AddComponent<EffectBase>();
        effect.Initalize();
        return effect;
    }

    public void SpawnAndRemove(Vector3 pos, string effectName, float dir = 1f) {
        EffectBase effect = _effectPool.GetObject();

        System.Action callback = () => { _effectPool.ReturnObject(effect);};
        RuntimeAnimatorController controller = _resourceManager.GetAnimatorController(effectName);
        effect.SetEffectInfo(pos, controller, dir, callback);

        _acitveEffects.Add(effect);
    }

    public void SpawnEffect(Vector3 pos, string effectName, System.Action onEffectEnd, float dir = 1f) {
        EffectBase effect = _effectPool.GetObject();
        RuntimeAnimatorController controller = _resourceManager.GetAnimatorController(effectName);
        effect.SetEffectInfo(pos, controller, dir, onEffectEnd);

        _acitveEffects.Add(effect);
    }
}
