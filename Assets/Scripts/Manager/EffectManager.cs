﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class EffectManager : SingletonWithMonoBehaviour<EffectManager>
{
    private SpriteAtlas _effectAtlas;
    private ObjectPool<EffectBase> _effectPool;
    private AssetLoader _assetLoader;
    private List<EffectBase> _acitveEffects;
    private CameraShake _cameraShake;
    public delegate bool IsEffectEnd();

    public void Initialize() {
        _assetLoader = AssetLoader.GetInstance();
        _effectAtlas = Resources.Load<SpriteAtlas>("Atlas/EffectAtlas");
        _effectPool = new ObjectPool<EffectBase>(20, CreateEffect);
        _acitveEffects = new List<EffectBase>();
        _cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    public void Progress() {
        for (int i = 0; i < _acitveEffects.Count; i++) {
            _acitveEffects[i].Progress(_effectAtlas);
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

    public void SpawnParticle(Vector3 pos, string effectName, float angle, float angleRate, float dir, float speed, float gravity, float friction, float scale, float scaleChange, float lifeTime, SpriteAtlasAnimator.OnAnimationEnd onEnd = null) {
        EffectBase effect = _effectPool.GetObject();
        Transform et = effect.transform;
        et.localScale = Vector3.one * scale;
        Vector3 localScaleChange = Vector3.one * scaleChange;
        et.localRotation = Aroma.RotationUtility.ChangeAngle(angle);

        Vector3 gravityVec = Vector3.down * gravity;
        float radian = dir * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;

        System.Action onEffectUpdate = () => { 
            float currentFriction = (Mathf.Abs(speed) < Mathf.Epsilon) ? 0f : friction;

            et.position += direction * speed * Time.deltaTime;
            et.position -= direction * currentFriction * Time.deltaTime;
            et.position -= gravityVec * Time.deltaTime;
            
            float lastAngle = et.localRotation.z;
            et.localRotation = Aroma.RotationUtility.ChangeAngle(lastAngle + angleRate);
            et.localScale += localScaleChange;
        };

        SpriteAtlasAnimator.OnAnimationEnd endCallback = onEnd;
        endCallback += () => { 
            effect.transform.localScale = Vector3.one;
           _effectPool.ReturnObject(effect);
        };

        effect.SetEffectInfoWithDuration(pos, effectName, dir, lifeTime, onEffectUpdate, endCallback);

        _acitveEffects.Add(effect);
    }

    #region custom particle scripts
    public void SpawnParticleFire(Vector3 pos) {
        var _r = Random.Range(0f,360f);
        SpawnParticle(pos,"EFFECT_Fire",_r,3,_r,Random.Range(26,32),-6,24,0.5f,0.08f,Random.Range(20,60));
    }
    #endregion

    public void SpawnEffect(Vector3 pos, string effectName, System.Action onEffectUpdate, SpriteAtlasAnimator.OnAnimationEnd onEffectEnd, float dir = 1f) {
        EffectBase effect = _effectPool.GetObject();
        effect.SetEffectInfo(pos, effectName, dir, onEffectEnd);

        _acitveEffects.Add(effect);
    }

    public void SpawnEffectWithCondition(Vector3 pos, string effectName, IsEffectEnd isEffectEnd, SpriteAtlasAnimator.OnAnimationEnd onEffectEnd, float dir = 1f) {
        EffectBase effect = _effectPool.GetObject();

        onEffectEnd += () => { _effectPool.ReturnObject(effect);};

        effect.SetEffectInfoWithCondition(pos, effectName, dir, null, isEffectEnd, onEffectEnd);

        _acitveEffects.Add(effect);
    }

    public void SpawnTrackEffectWithCondition(Vector3 pos, string effectName, IsEffectEnd isEffectEnd, Transform target, SpriteAtlasAnimator.OnAnimationEnd onEffectEnd, float dir = 1f) {
        EffectBase effect = _effectPool.GetObject();

        onEffectEnd += () => { _effectPool.ReturnObject(effect);};

        System.Action onEffectUpdate = () => { effect.transform.position = target.position; };
        effect.SetEffectInfoWithCondition(pos, effectName, dir, onEffectUpdate, isEffectEnd, onEffectEnd);

        _acitveEffects.Add(effect);
    }

    public void SpawnEffectWithDuration(Vector3 pos, string effectName, float duration, SpriteAtlasAnimator.OnAnimationEnd onEffectEnd, float dir = 1f) {
        EffectBase effect = _effectPool.GetObject();

        onEffectEnd += () => { _effectPool.ReturnObject(effect);};
        effect.SetEffectInfoWithDuration(pos, effectName, dir, duration, onEffectEnd);

        _acitveEffects.Add(effect);
    }

    public void SpawnEffectWithDuration(Vector3 pos, string effectName, float duration, System.Action updateCallback, SpriteAtlasAnimator.OnAnimationEnd onEffectEnd, float dir = 1f) {
        EffectBase effect = _effectPool.GetObject();

        onEffectEnd += () => { _effectPool.ReturnObject(effect);};
        effect.SetEffectInfoWithDuration(pos, effectName, dir, duration, updateCallback, onEffectEnd);

        _acitveEffects.Add(effect);
    }
}
