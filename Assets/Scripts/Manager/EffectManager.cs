using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    ObjectPool<EffectBase> _effectPool;

    public void Initalize() {
        
    }
}
