#pragma warning disable 0618
/*
<copyright file="BGDatabaseExcelImportEditorTool.cs" company="BansheeGz">
    Copyright (c) 2019 All Rights Reserved
</copyright>
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace BansheeGz.BGDatabase.Editor
{
    //this file adds Excel file monitoring for Unity Editor
    [InitializeOnLoad]
    public static class BGDatabaseExcelImportEditorTool
    {
        private const string Key_ExcelOn = "BGDatabase.Custom.ExcelOn";
        private const string Key_ExcelFilePath = "BGDatabase.Custom.ExcelFilePath";
        private const string Key_ExcelDelay = "BGDatabase.Custom.ExcelDelay";
        private const string Key_ExcelLastSynced = "BGDatabase.Custom.ExcelLastSynced";
        private const string Key_ExcelSaveDatabase = "BGDatabase.Custom.ExcelSaveDatabase";
        private const string Key_ExcelDebug = "BGDatabase.Custom.ExcelDebug";

        private static readonly ConcurrentQueue<Action> TasksQueue = new ConcurrentQueue<Action>();
        private static ThreadNotifier threadNotifier;

        private static bool excelOn;
        private static int excelDelay = 2000;
        private static string excelFilePath;
        private static long excelLastSynced;
        private static bool excelSaveDatabase;
        private static bool excelDebug;


        public static bool ExcelOn
        {
            set
            {
                if (value == excelOn) return;
                SetBool(Key_ExcelOn, ref excelOn, value);

                if (value) StartBackgroundThread();
                else StopBackgroundThread();
            }
        }

        public static int ExcelDelay
        {
            set
            {
                value = Mathf.Max(200, EditorPrefs.GetInt(Key_ExcelDelay));
                SetInt(Key_ExcelDelay, ref excelDelay, value);
            }
        }


        public static string ExcelFilePath
        {
            set
            {
                if (string.Equals(excelFilePath, value)) return;
                SetString(Key_ExcelFilePath, ref excelFilePath, value);
            }
        }


        public static long ExcelLastSynced
        {
            set
            {
                excelLastSynced = value;
                RunOnMainThread(() => EditorPrefs.SetString(Key_ExcelLastSynced, "" + value));
            }
        }

        public static bool ExcelSaveDatabase
        {
            set { SetBool(Key_ExcelSaveDatabase, ref excelSaveDatabase, value); }
        }


        public static bool ExcelDebug
        {
            set { SetBool(Key_ExcelDebug, ref excelDebug, value); }
        }


        static BGDatabaseExcelImportEditorTool()
        {
            EditorApplication.update += ThisIsExecutedOnMainThread;

            RunOnMainThread(() =>
            {
                excelOn = EditorPrefs.GetBool(Key_ExcelOn);
                excelDelay = Mathf.Max(200, EditorPrefs.GetInt(Key_ExcelDelay));
                excelLastSynced = ReadStringAsLong(Key_ExcelLastSynced);
                excelFilePath = EditorPrefs.GetString(Key_ExcelFilePath);
                excelSaveDatabase = EditorPrefs.GetBool(Key_ExcelSaveDatabase);
                excelDebug = EditorPrefs.GetBool(Key_ExcelDebug);

                if (excelOn) StartBackgroundThread();
            });
        }

        private static void ThisIsExecutedOnMainThread()
        {
            if (TasksQueue.Count > 10)
            {
                Debug.LogError("Unexpected behavior: task queue has more than 10 tasks! Emergency exit: all tasks are canceled !");
                TasksQueue.Clear();
                return;
            }

            Action task;
            while ((task = TasksQueue.Dequeue()) != null)
            {
                try
                {
                    task();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private static void StartBackgroundThread()
        {
            if (threadNotifier != null) StopBackgroundThread();
            threadNotifier = new ThreadNotifier();
            var thread = new Thread(ThisIsBackgroundWorker) {IsBackground = true};
            thread.Start(threadNotifier);
        }

        private static void StopBackgroundThread()
        {
            if (threadNotifier != null) threadNotifier.ExitRequested = true;
        }

        private static void RunOnMainThread(Action action)
        {
            TasksQueue.Enqueue(action);
        }

        private static void ThisIsBackgroundWorker(object notifier)
        {
            Thread.Sleep(500);

            var threadNotifier = (ThreadNotifier) notifier;
            while (true)
            {
                if (threadNotifier.ExitRequested) break;
                Thread.Sleep(excelDelay);
                if (threadNotifier.ExitRequested) break;

                if (!excelOn) continue;

                if (excelFilePath == null || !File.Exists(excelFilePath)) continue;

                var modified = File.GetLastWriteTime(excelFilePath);
                if (modified.Ticks != excelLastSynced)
                {
                    ExcelLastSynced = modified.Ticks;
                    RunOnMainThread(ImportExcel);
                }
            }
        }

        private static void ImportExcel()
        {
            var watch = Stopwatch.StartNew();

            new BGExcel(new BGLogger(), BGRepo.I, new BGMergeSettingsEntity {UpdateMatching = true}, new BGMergeSettingsMeta()).Import(excelFilePath, false);
            if (excelSaveDatabase)
            {
                BGRepoSaver.SaveRepo();
                BGRepo.I.Events.FireAnyChange();
                if (BGRepoWindow.Active) BGRepoWindow.Instance.MarkRepoSaved();
            }

            ForEach<BGDataBinderRowGo>(go => go.Bind());
            ForEach<BGDataBinderDatabaseGo>(go => go.Bind());


            watch.Stop();
            if (excelDebug) Debug.Log("ExcelFileMonitor: executed Excel import in " + watch.Elapsed.Milliseconds + " mls.");
        }

/*
    private static void TestT(BGDBAutoMapRegistry.AutoMappedConfig autoMappedConfig, BGEntity entity, Object target)
    {
        autoMappedConfig.ForEachField((id, info) =>
        {
            var field = entity.Meta.GetField(id, false);
            if (field != null)
            {
                try
                {
                    var value = field.GetValue(entity.Index);
                    info.SetValue((object) target, value);
                }
                catch (Exception ex)
                {
                    Debug.LogError("DataBinder error: details are [From Meta=" + entity.Meta.Name + ", field=" + field.Name + ", entity=" + entity.Name + "] [To Class=" + target.GetType().Name + "]  Original error is: " + ex.Message);
                }
            }
            
        });
    }
*/


        private static void ForEach<T>(Action<T> action) where T : MonoBehaviour
        {
            var list = Object.FindObjectsOfType<T>();
            if (list == null) return;
            foreach (var o in list) action(o);
        }

        [MenuItem("Window/BGDatabaseExcelTools")]
        public static void Open()
        {
            ToolsWindow.Instance.Show();
        }

        private static void SetBool(string key, ref bool b, bool value)
        {
            b = value;
            EditorPrefs.SetBool(key, value);
        }

        private static void SetInt(string key, ref int b, int value)
        {
            b = value;
            EditorPrefs.SetInt(key, value);
        }

        private static void SetString(string key, ref string b, string value)
        {
            b = value;
            EditorPrefs.SetString(key, value);
        }

        private static void SetFloat(string key, ref float b, float value)
        {
            b = value;
            EditorPrefs.SetFloat(key, value);
        }

        private static long ReadStringAsLong(string key)
        {
            var value = EditorPrefs.GetString(key);
            if (string.IsNullOrEmpty(value)) return 0;
            try
            {
                return long.Parse(value);
            }
            catch
            {
                return 0;
            }
        }

        private class ThreadNotifier
        {
            public bool ExitRequested;
        }

        private class ToolsWindow : EditorWindow
        {
            public static ToolsWindow Instance
            {
                get { return GetWindow<ToolsWindow>(null, false); }
            }

            private void OnGUI()
            {
                BGEditorUtility.TabPanel("Excel file monitor", () =>
                {
                    BGEditorUtility.Toggle(new GUIContent("On", "Is background monitor is on"), excelOn, v => ExcelOn = v);
                    if (!excelOn) GUILayout.Label("Monitor is off", BGStyle.Attention);


                    BGEditorUtility.Horizontal(() =>
                    {
                        GUILayout.Label("File");
                        ExcelFilePath = GUILayout.TextField(excelFilePath);
                        if (BGEditorUtility.Button("Open", 80))
                        {
                            BGEditorUtility.OpenFile("Excel file to monitor", null, null, s => ExcelFilePath = s);
                        }
                    });

                    BGEditorUtility.Horizontal(() =>
                    {
                        GUILayout.Label("Scan Period in milliseconds (min=200)");
                        var num = EditorGUILayout.DelayedIntField(excelDelay);
                        if (excelDelay != num) ExcelDelay = num;
                    });

                    BGEditorUtility.Toggle(new GUIContent("Auto-Save database", "If this setting is on, all changes to BGDatabase from Excel file will be auto-saved"), excelSaveDatabase,
                        v => ExcelSaveDatabase = v);


                    BGEditorUtility.Toggle(new GUIContent("Debug", "Turn it on to see how much time excel import has taken in the console"),
                        excelDebug, v => ExcelDebug = v);

                    BGEditorUtility.SwapDisabled(() =>
                    {
                        var fileIsSet = !string.IsNullOrEmpty(excelFilePath);
                        var fileExists = fileIsSet && File.Exists(excelFilePath);

                        EditorGUILayout.LabelField("File in use :", fileIsSet ? excelFilePath : "N/A");
                        EditorGUILayout.LabelField("File exists? :", fileExists ? "Yes" : "No",
                            fileExists ? new GUIStyle {normal = {textColor = new Color(0f, 0.64f, 0.15f)}} : new GUIStyle {normal = {textColor = Color.red}});

                        EditorGUILayout.LabelField("File last modified :", !fileExists ? "N/A" : File.GetLastWriteTime(excelFilePath).ToString("MM/dd/yyyy HH:mm:ss"));
                        EditorGUILayout.LabelField("Last synced :", excelLastSynced == 0 ? "N/A" : new DateTime(excelLastSynced).ToString("MM/dd/yyyy HH:mm:ss"));
                    }, !excelOn);

                    BGEditorUtility.SwapDisabled(() =>
                    {
                        if (!BGEditorUtility.Button("Run now")) return;
                        ImportExcel();
                    }, excelOn);
                });
            }
        }
        
        //original from https://stackoverflow.com/questions/4555307/enabling-queuet-with-concurrency
        private class ConcurrentQueue<T> : ICollection, IEnumerable<T>
        {
            private readonly Queue<T> _queue;

            public ConcurrentQueue()
            {
                _queue = new Queue<T>();
            }

            public IEnumerator<T> GetEnumerator()
            {
                lock (SyncRoot)
                {
                    foreach (var item in _queue)
                    {
                        yield return item;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void CopyTo(Array array, int index)
            {
                lock (SyncRoot)
                {
                    ((ICollection)_queue).CopyTo(array, index);
                }
            }

            public int Count
            {
                get
                { 
                    // Assumed to be atomic, so locking is unnecessary
                    return _queue.Count;
                }
            }

            public object SyncRoot
            {
                get { return ((ICollection)_queue).SyncRoot; }
            }

            public bool IsSynchronized
            {
                get { return true; }
            }

            public void Enqueue(T item)
            {
                lock (SyncRoot)
                {
                    _queue.Enqueue(item);
                }
            }

            public T Dequeue()
            {
                lock(SyncRoot)
                {
                    return _queue.Count == 0 ? default(T) : _queue.Dequeue();
                }
            }

            public T Peek()
            {
                lock (SyncRoot)
                {
            
                    return _queue.Count == 0 ? default(T) : _queue.Peek();
                }
            }

            public void Clear()
            {
                lock (SyncRoot)
                {
                    _queue.Clear();
                }
            }
        }
    }
}