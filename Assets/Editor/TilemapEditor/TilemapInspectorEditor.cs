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
    private static int _roomNumber;

    private void OnEnable() {
        _context = (TilemapEditorScript)target;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current Room Number");
        _roomNumber = EditorGUILayout.IntField(_roomNumber);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(2f);

        if (GUILayout.Button("Export")) {
            if (EditorUtility.DisplayDialog("Warning", "Are you sure? The RoomBase will be overlaped!", "Export", "Do Not Export")) {
                var asset = _context.RequestExport();
                asset.roomNumber = _roomNumber;
                AssetDatabase.CreateAsset(asset, "Assets/Resources/Rooms/" + _roomNumber + ".asset");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        if (GUILayout.Button("Clear All")) {
            if (EditorUtility.DisplayDialog("Warning", "Are you sure?", "Clear", "Do Not Clear")) {
                _context.ClearAll();
            }
        }

        if (GUILayout.Button("Clear Tilemaps")) {
            if (EditorUtility.DisplayDialog("Warning", "Are you sure?", "Clear", "Do Not Clear")) {
                _context.ClearAllTilemaps();
            }
        }

        if (GUILayout.Button("Clear Objects")) {
            if (EditorUtility.DisplayDialog("Warning", "Are you sure?", "Clear", "Do Not Clear")) {
                _context.ClearAllObjects();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
