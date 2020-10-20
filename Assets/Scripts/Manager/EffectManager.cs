using System.Collections;
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

    public void SpawnParticle(Vector3 pos, string effectName, float angle, float angleRate, float dir, float speed, float gravity, float friction, float lifeTime, float scale = 1f, float scaleRate = 0f, SpriteAtlasAnimator.OnAnimationEnd onEnd = null) {
        EffectBase effect = _effectPool.GetObject();
        Transform et = effect.transform;
        et.localScale = Vector3.one * scale;
        Vector3 scaleRateVec = Vector3.one * scaleRate;
        et.localRotation = Aroma.RotationUtility.ChangeAngle(angle);

        Vector3 gravityVec = Vector3.down * gravity;
        float radian = dir * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;

        effect.Speed = speed;
        System.Action onEffectUpdate = () => { 
            float currentFriction = (Mathf.Abs(speed) < Mathf.Epsilon) ? 0f : friction;
            effect.Speed -= currentFriction * Time.deltaTime * 10f;
            if (effect.Speed < Mathf.Epsilon)
                effect.Speed = 0f;

            et.position += direction * effect.Speed * Time.deltaTime;
            et.position += gravityVec * Time.deltaTime;

            float lastAngle = et.localRotation.eulerAngles.z;
            et.localRotation = Aroma.RotationUtility.ChangeAngle(lastAngle + angleRate);

            Vector3 nextScale = et.localScale + scaleRateVec * Time.deltaTime;
            if (nextScale.x < Mathf.Epsilon || nextScale.y < Mathf.Epsilon) {
                et.localScale = Vector3.zero;
                return;
            }
            et.localScale = nextScale;
        };

        SpriteAtlasAnimator.OnAnimationEnd endCallback = onEnd;
        endCallback += () => { 
            et.localScale = Vector3.one;
           _effectPool.ReturnObject(effect);
        };

        effect.SetEffectInfoWithDuration(pos, effectName, Mathf.Sign(direction.x), lifeTime, false, onEffectUpdate, endCallback);

        _acitveEffects.Add(effect);
    }

    #region custom particle scripts
    public void SpawnParticleFire(Vector3 pos) {
        float r = Random.Range(0f, 360f);
        SpawnParticle(pos, "EFFECT_Fire", r, 3f, 90f+Random.Range(-30f,30f), Random.Range(0f, 3f), -1f, 4f, Random.Range(1f ,2f), 0.8f, -1f);
    }
    public void SpawnParticleSnowflake(Vector3 pos) {
        float r = Random.Range(0f, 360f);
        Vector3 newPos = GetRandomVector3(pos,0.8f);
        SpawnParticleCircle(GetRandomVector3(newPos,0.4f));
        SpawnParticle(newPos, "EFFECT_SnowFlake", r, 0.3f, 90f+Random.Range(-60f,60f), 0.8f, 0.5f, 0.6f, Random.Range(1.6f ,2.4f), Random.Range(0.8f,1f), -0.5f);
    }
    public void SpawnParticleCircle(Vector3 pos) {
        float r = Random.Range(0f, 360f);
        Vector3 newPos = GetRandomVector3(pos,0.8f);
        SpawnParticle(newPos, "EFFECT_SnowSmoke", r, 0.3f, 90f+Random.Range(-60f,60f), 0.8f, 0.3f, 0.6f, Random.Range(2f ,3f), Random.Range(0.3f,0.4f), -0.5f);
    }
    private Vector3 GetRandomVector3(Vector3 vector, float range) {
        Vector3 newVector = new Vector3(Random.Range(vector.x-range,vector.x+range),Random.Range(vector.y-range,vector.y+range),0);
        return newVector;
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

        effect.SetEffectInfoWithCondition(pos, effectName, dir, true, null, isEffectEnd, onEffectEnd);

        _acitveEffects.Add(effect);
    }

    public void SpawnTrackEffectWithCondition(Vector3 pos, string effectName, IsEffectEnd isEffectEnd, Transform target, SpriteAtlasAnimator.OnAnimationEnd onEffectEnd, float dir = 1f) {
        EffectBase effect = _effectPool.GetObject();

        onEffectEnd += () => { _effectPool.ReturnObject(effect);};

        System.Action onEffectUpdate = () => { effect.transform.position = target.position; };
        effect.SetEffectInfoWithCondition(pos, effectName, dir, true, onEffectUpdate, isEffectEnd, onEffectEnd);

        _acitveEffects.Add(effect);
    }

    public void SpawnEffectWithDuration(Vector3 pos, string effectName, float duration, SpriteAtlasAnimator.OnAnimationEnd onEffectEnd, float dir = 1f) {
        EffectBase effect = _effectPool.GetObject();

        onEffectEnd += () => { _effectPool.ReturnObject(effect);};
        effect.SetEffectInfoWithDuration(pos, effectName, dir, duration, true, onEffectEnd);

        _acitveEffects.Add(effect);
    }

    public void SpawnEffectWithDuration(Vector3 pos, string effectName, float duration, System.Action updateCallback, SpriteAtlasAnimator.OnAnimationEnd onEffectEnd, float dir = 1f) {
        EffectBase effect = _effectPool.GetObject();

        onEffectEnd += () => { _effectPool.ReturnObject(effect);};
        effect.SetEffectInfoWithDuration(pos, effectName, dir, duration, true, updateCallback, onEffectEnd);

        _acitveEffects.Add(effect);
    }
}
