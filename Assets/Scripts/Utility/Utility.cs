using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

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
        public static Vector2 ChangeXPos(this Vector2 vec, float x) {
            Vector2 newVec = new Vector2(x, vec.y);
            return newVec;
        }

        public static Vector3 ChangeXPos(this Vector3 vec, float x) {
            Vector3 newVec = new Vector3(x, vec.y, vec.z);
            return newVec;
        }
    }

    public static class GameObjectExtensions
    {
        private static List<Component> _componentCache = new List<Component>();

        public static T GetComponentNoAlloc<T>(this GameObject obj) where T : Component
        {
            obj.GetComponents(typeof(T), _componentCache);
            var component = _componentCache.Count > 0 ? _componentCache[0] : null;
            _componentCache.Clear();
            return component as T;
        }
    }

    public static class EnemyUtility {
        public static EnemyTypes NameToEnemy(string name) {
            return (EnemyTypes)System.Enum.Parse(typeof(EnemyTypes), name);
        }

        public static EnemyTypes ObjToEnemy(EnemyBase enemy) {
            return NameToEnemy(enemy.EnemyName);
        }
    }

    public static class StringUtils {
        private static StringBuilder _stringBuilder = new StringBuilder(64);
        public static string MergeStrings(params string[] strList) {
            _stringBuilder.Clear();
            foreach (string str in strList) {
                _stringBuilder.Append(str);
            }
            return _stringBuilder.ToString();
        }
    }
}
