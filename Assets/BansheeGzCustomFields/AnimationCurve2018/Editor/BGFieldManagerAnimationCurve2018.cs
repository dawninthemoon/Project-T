/*
<copyright file="BGFieldManagerAnimationCurve2018.cs" company="BansheeGz">
    Copyright (c) 2018-2020 All Rights Reserved
</copyright>
*/

using UnityEditor;
using UnityEngine;

namespace BansheeGz.BGDatabase.Editor
{
    public class BGFieldManagerAnimationCurve2018 : BGFieldManagerUnityClassInlinedA<AnimationCurve>
    {
        protected override AnimationCurve Edit(Rect position, AnimationCurve value)
        {
            return EditorGUI.CurveField(position, value);
        }
    }
}