using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(TilemapEditorScript)), CanEditMultipleObjects]
public class TilemapInspectorEditor : Editor
{
    private TilemapEditorScript _context;

    private void OnEnable() {
        _context = (TilemapEditorScript)target;
    }

    public override void OnInspectorGUI() {
        if (GUILayout.Button("Export")) {
            _context.Export();
        }
    }
}
