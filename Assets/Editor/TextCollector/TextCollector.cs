using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OS.Text
{
    //Scene上のすべてのUI.Textの文字列を取得しSceneで使用されている文字列を特定する
    public class TextCollector
    {

        public string CollectUsingText(Scene scene,SortedSet<char> oldSet=null,  bool print = false,bool CopyToClipBoard=true)
        {
            return CollectUsingText(new[] {scene}, oldSet,print,CopyToClipBoard);
        }

        public string CollectUsingText(Scene[] scene,SortedSet<char> oldSet, bool print = false,bool CopyToClipBoard=true)
        {
            SortedSet<char> set = oldSet;
            if(set==null)set =new SortedSet<char>();
            StringBuilder allBuilder = new StringBuilder();

            if (scene == null)
                GetTextFromAllScene(allBuilder);
            else
                foreach (var s in scene)
                    GetTextFromScene(s.GetRootGameObjects(), allBuilder);

            allBuilder.ToString().ToCharArray().ToList()
                .ForEach(c =>
                {
                    if (!set.Contains(c)) set.Add(c);
                });

            allBuilder.Clear();
            set.ToList().ForEach(c => allBuilder.Append(c));
            var allCode = allBuilder.ToString();
            if (print) Debug.Log(allCode);

            if (CopyToClipBoard) GUIUtility.systemCopyBuffer = allCode;
            return allCode;
        }

        public string toString(SortedSet<char> set)
        {
            var builder =new StringBuilder();
            int i = 0;
            set.ToList().ForEach(c =>
            {
                i++;
                if (i == 30)
                {
                    i = 0;
                    builder.AppendLine();
                }
                builder.Append(c);
            });
            return builder.ToString();
        }

        /// <summary>
        /// BuildSettingに登録されているシーンのText要素を抽出する
        /// </summary>
        private void GetTextFromAllScene(StringBuilder builder)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scene = SceneManager.GetSceneByBuildIndex(i);
                GetTextFromScene(scene.GetRootGameObjects(), builder);
            }
        }

        private void GetTextFromScene(GameObject[] rootObjects, StringBuilder builder)
        {
            foreach (var g in rootObjects) SearchChildren(g, builder);
        }

        private void SearchChildren(GameObject g, StringBuilder builder)
        {
            if (g.transform.childCount == 0) return;
            Transform[] ch = new Transform[g.transform.childCount];
            for (int i = 0; i < g.transform.childCount; i++)
            {
                ch[i] = g.transform.GetChild(i);
                if (ch[i].GetComponent<UnityEngine.UI.Text>())
                    builder.Append(ch[i].GetComponent<UnityEngine.UI.Text>().text);
                if (ch[i].GetComponent<TextMeshProUGUI>()) builder.Append(ch[i].GetComponent<TextMeshProUGUI>().text);
                if (ch[i].GetComponent<TextMesh>()) builder.Append(ch[i].GetComponent<TextMesh>().text);
                if (ch[i].GetComponent<TextMeshPro>()) builder.Append(ch[i].GetComponent<TextMeshPro>().text);
                
                SearchChildren(ch[i].gameObject, builder);
            }
        }
    }
}