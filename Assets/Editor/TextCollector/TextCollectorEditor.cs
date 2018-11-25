using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace OS.TextCollector
{
    public class TextCollectorEditor : EditorWindow
    {
        private const string savePath = "Assets/Editor/TextCollector/data.asset";

        [MenuItem("Window/TextCollector")]
        static void Open()
        {
            var pref = AssetDatabase.LoadAssetAtPath<TextCollectorParams>(savePath);

            var tce = GetWindow<TextCollectorEditor>();
            if (pref == null)
                pref = CreateInstance<TextCollectorParams>();
            tce._params = pref;
        }

        private void OnDestroy()
        {
            AssetDatabase.CreateAsset(_params, savePath);
            Collector = null;
        }

        private void OnDisable()
        {
            if (Collector == null) Collector = new TextCollector();
        }

        public TextCollector Collector;
        private TextCollectorParams _params;
        private Vector2 _scroll;

        private void OnGUI()
        {
            if (Collector == null) Collector = new TextCollector();
            EditorGUILayout.BeginVertical();
            _params.PrintText = EditorGUILayout.Toggle("Print Text", _params.PrintText);
            _params.LoadAll = EditorGUILayout.Toggle("Load AllScene", _params.LoadAll);

            if (_params.LoadAll) EditorGUILayout.LabelField("Warning ! Before Load Save Current Scene");
            else EditorGUILayout.LabelField($"Load Current Scene {EditorSceneManager.GetActiveScene().name}");


            EditorGUILayout.LabelField("Append Chars");
            _params.AppendChars = EditorGUILayout.TextArea(_params.AppendChars);

            if (_params.LoadAll)
            {
                if (GUILayout.Button("Collect From All Scene"))
                {
//                    var activeScene = SceneManager.GetActiveScene();
//                    Debug.Log("Active Scene:"+activeScene.name);
                    var set = new SortedSet<char>();
                    Append(set);
                    foreach (var s in EditorBuildSettings.scenes)
                    {
                        var scene = EditorSceneManager.OpenScene(s.path);
                        Collector.CollectUsingText(scene, set, print: _params.PrintText);
                    }

                    _params.chars = Collector.toString(set);
//                    EditorSceneManager.OpenScene(activeScene.name);
                }
            }
            else
            {
                if (GUILayout.Button("Collect From Selected Scene"))
                {
                    var set = new SortedSet<char>();
                    Append(set);
                    var activeScene = EditorSceneManager.GetActiveScene();
                    Collector.CollectUsingText(activeScene, set, print: _params.PrintText);
                    _params.chars = Collector.toString(set);
                }
            }
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            _params.chars = EditorGUILayout.TextArea(_params.chars, GUILayout.Height(position.height - 80));
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void Append(SortedSet<char> set)
        {
            foreach (var c in _params .AppendChars.ToCharArray())
                if (!set.Contains(c))
                    set.Add(c);
        }
    }
}