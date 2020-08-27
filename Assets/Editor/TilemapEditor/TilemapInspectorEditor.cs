using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace LevelEditor {
    [CustomEditor(typeof(TilemapEditorScript)), CanEditMultipleObjects]
    public class TilemapInspectorEditor : Editor
    {
        private TilemapEditorScript _context;
        public static int RoomNumber;

        private List<SORoomBase> roomBases;

        private void OnEnable() {
            _context = (TilemapEditorScript)target;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current Room Number");
            RoomNumber = EditorGUILayout.IntField(RoomNumber);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5f);

            var defaultColor = GUI.backgroundColor;

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Apply Changes", GUILayout.Height(50f))) {
                BuildAsssetBundles.BuildRoomAssetBundles();
            }

            EditorGUILayout.Space(5f);

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Import", GUILayout.Height(40f))) {
                var rooms = GetAllRooms();

                ImportRoomBaseWindow.OpenWindow();
            }

            if (GUILayout.Button("Export", GUILayout.Height(40f))) {
                if (EditorUtility.DisplayDialog("Warning", "Are you sure? The RoomBase will be overlaped!", "Export", "Do Not Export")) {
                    Export();
                        
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            EditorGUILayout.Space(5f);

            GUI.backgroundColor = defaultColor;
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

        private void Export() {
            var asset = _context.RequestExport();
            asset.roomNumber = RoomNumber;
            string path = "Assets/ScriptableObjects/Rooms/Room " + RoomNumber + ".asset";
            AssetDatabase.CreateAsset(asset, path);
            
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            assetImporter.SetAssetBundleNameAndVariant("assetbundle_0", "");

            //BuildAsssetBundles.BuildNeedAssetBundle("assetbundle_0");
        }

        public static List<SORoomBase> GetAllRooms() {
            List<SORoomBase> rooms = new List<SORoomBase>();

            string[] allRoomBaseFiles = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);
            foreach(string roomFile in allRoomBaseFiles)
            {
                string assetPath = "Assets" + roomFile.Replace(Application.dataPath, "").Replace('\\', '/');
                SORoomBase source = AssetDatabase.LoadAssetAtPath(assetPath, typeof(SORoomBase)) as SORoomBase;
                if (source) {
                    rooms.Add(source);
                }
            }

            return rooms;
        }
    }
}