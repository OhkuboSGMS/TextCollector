using UnityEngine;

namespace OS.TextCollector
{
    public class TextCollectorParams : ScriptableObject
    {
        public string chars;
        public string AppendChars;

        public bool PrintText;
        public bool LoadAll;
    }
}