using System;
using System.Collections.Generic;
using OS.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace OS.Text
{
    public class TextCollectorEditor : EditorWindow
    {
        [MenuItem("Window/TextCollector")]
        static void Open()
        {
            GetWindow<TextCollectorEditor>();
        }

        private void OnDestroy()
        {
            Collector = null;
        }

        private void OnDisable()
        {
            if (Collector == null) Collector = new TextCollector();
        }

        public TextCollector Collector;
        public string chars = String.Empty;
        public string AppendChars = String.Empty;

        public bool PrintText;
        public bool LoadAll;
        private Vector2 _scroll;

        private void OnGUI()
        {
            if (Collector == null) Collector = new TextCollector();
            EditorGUILayout.BeginVertical();
            PrintText = EditorGUILayout.Toggle("Print Text", PrintText);
            LoadAll = EditorGUILayout.Toggle("Load AllScene", LoadAll);

            if (LoadAll) EditorGUILayout.LabelField("Warning ! Before Load Save Current Scene");
            else EditorGUILayout.LabelField($"Load Current Scene {EditorSceneManager.GetActiveScene().name}");


            EditorGUILayout.LabelField("Append Chars");
            AppendChars =EditorGUILayout.TextArea(AppendChars);
            
            if (LoadAll)
            {
                if (GUILayout.Button("Collect From All Scene"))
                {
//                    var activeScene = SceneManager.GetActiveScene();
//                    Debug.Log("Active Scene:"+activeScene.name);
                    var set = new SortedSet<char>();
                    Apeend(set);
                    foreach (var s in EditorBuildSettings.scenes)
                    {
                        var scene = EditorSceneManager.OpenScene(s.path);
                        Collector.CollectUsingText(scene, set, print: PrintText);
                    }
                    chars = Collector.toString(set);
//                    EditorSceneManager.OpenScene(activeScene.name);
                }
            }
            else
            {
                if (GUILayout.Button("Collect From Selected Scene"))
                {
                    var set = new SortedSet<char>();
                    Apeend(set);
                    var activeScene = EditorSceneManager.GetActiveScene();
                    Collector.CollectUsingText(activeScene, set,print: PrintText);
                    chars = Collector.toString(set);
                }
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            chars = EditorGUILayout.TextArea(chars, GUILayout.Height(position.height-80));
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void Apeend(SortedSet<char> set)
        {
            foreach (var c in AppendChars.ToCharArray())
                if (!set.Contains(c))
                    set.Add(c);
        }
    }
}