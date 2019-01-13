using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sweet.BuildTools.Editor
{
    [CustomPropertyDrawer(typeof(WebGLTemplateFieldAttribute))]
    public class WebGLTemplateFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string[] templateNames;
            string[] templateValues;

            GetTemplates(out templateNames, out templateValues);

            if (templateNames.Length == 0)
            {
                EditorGUI.Popup(position, "Template", 0, new [] { "None" });
            }

            string currentValue = property.stringValue;
            int currentIndex = Array.FindIndex(templateValues, v => v == currentValue);

            if (currentIndex == -1)
            {
                currentIndex = Array.FindIndex(templateValues, v => v == "APPLICATION:Default");

                if (currentIndex == -1)
                {
                    currentIndex = 0;
                }
            }

            currentIndex = EditorGUI.Popup(position, "Template", currentIndex, templateNames);
            property.stringValue = templateValues[currentIndex];
        }


        private static void GetTemplates(out string[] names, out string[] values)
        {
            var namesList = new List<string>();
            var valuesList = new List<string>();
            
            string standardTemplatesFolder = GetStandardTemplatesFolder();
            AddTemplatesAtPath(namesList, valuesList, standardTemplatesFolder, "APPLICATION");
            AddTemplatesAtPath(namesList, valuesList, Path.GetFullPath("Assets/WebGLTemplates"), "PROJECT");

            names = namesList.ToArray();
            values = valuesList.ToArray();
        }

        private static string GetStandardTemplatesFolder()
        {
#if UNITY_EDITOR_WIN
            return EditorApplication.applicationContentsPath + "/PlaybackEngines/WebGLSupport/BuildTools/WebGLTemplates";
#elif UNITY_EDITOR_OSX
            return EditorApplication.applicationPath + "/../PlaybackEngines/WebGLSupport/BuildTools/WebGLTemplates";
#endif
        }

        private static void AddTemplatesAtPath(List<string> names, List<string> values, string rootPath, string prefix)
        {
            var directoryInfo = new DirectoryInfo(rootPath);
            
            if (!directoryInfo.Exists)
            {
                return;
            }

            DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();

            for (int i = 0; i < subDirectories.Length; i++)
            {
                DirectoryInfo d = subDirectories[i];
                names.Add(d.Name);
                values.Add(prefix + ":" + d.Name);
            }
        }
    }
}