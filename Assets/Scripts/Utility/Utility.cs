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

    public static class VectorUtils {
        public static Vector3 GetScaleVec(float dir) {
            Vector3 scale = new Vector3(dir, 1f, 1f);
            return scale;
        }
    }
}
