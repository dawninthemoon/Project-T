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
    private int _roomNumber;

    private void OnEnable() {
        _context = (TilemapEditorScript)target;
    }

    public override void OnInspectorGUI() {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current Room Number");
        _roomNumber = EditorGUILayout.IntField(_roomNumber);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(2f);

        if (GUILayout.Button("Export")) {
            var asset = _context.RequestExport();
            asset.roomNumber = _roomNumber;
            AssetDatabase.CreateAsset(asset, "Assets/Resources/Rooms/" + _roomNumber + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
}
