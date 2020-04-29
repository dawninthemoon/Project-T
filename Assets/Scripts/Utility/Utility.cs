using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aroma
{
    public static class CustomMath
    {
        public static float Ease(float x, float easeAmount)
        {
            float a = easeAmount + 1f;
            float xPowA = Mathf.Pow(x, a);
            return xPowA / (xPowA + Mathf.Pow(1 - x, a));
        }
    }

    public static class VectorUtility {
        public static Vector3 GetScaleVec(float dir) {
            Vector3 scale = new Vector3(dir, 1f, 1f);
            return scale;
        }
    }

    public static class GameObjectExtensions
    {
        static List<Component> _componentCache = new List<Component>();

        public static T GetComponentNoAlloc<T>(this GameObject obj) where T : Component
        {
            obj.GetComponents(typeof(T), _componentCache);
            var component = _componentCache.Count > 0 ? _componentCache[0] : null;
            _componentCache.Clear();
            return component as T;
        }
    }
}
