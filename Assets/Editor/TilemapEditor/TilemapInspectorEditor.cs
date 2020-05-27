using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LevelEditor {
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
            EditorGUILayout.LabelField("Door Prefab for Editor");
            _context.playerPointPrefab = EditorGUILayout.ObjectField(_context.playerPointPrefab, typeof(PlayerSpawnPosition), false) as PlayerSpawnPosition;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(2f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enemy Prefab for Editor");
            _context.enemyPointPrefab = EditorGUILayout.ObjectField(_context.enemyPointPrefab, typeof(EnemySpawnPosition), false) as EnemySpawnPosition;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(2f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("MovingPlatform Prefab for Editor");
            _context.movingPlatformPrefab = EditorGUILayout.ObjectField(_context.movingPlatformPrefab, typeof(PlatformController), false) as PlatformController;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current Room Number");
            _roomNumber = EditorGUILayout.IntField(_roomNumber);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(2f);

            if (GUILayout.Button("Import")) {
                string path = EditorUtility.OpenFilePanelWithFilters("Select RoomBase File", "Assets", new string[] { "ScriptableObject", "asset" });
                    if(path != string.Empty) {
                        int cutoffFrom = path.IndexOf("Assets");
                        path = path.Substring(cutoffFrom);
                        RoomBase roomBase = AssetDatabase.LoadAssetAtPath<RoomBase>(path) as RoomBase;
                        if (EditorUtility.DisplayDialog("Are you sure?", "Importing this room will overlap the current one without saving it.", "Okay", "Cancel"))
                        {
                            _context.Import(roomBase);
                        }	
                    }
            }
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
}