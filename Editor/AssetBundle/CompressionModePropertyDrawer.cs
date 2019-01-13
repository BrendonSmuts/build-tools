using System;
using UnityEditor;
using UnityEngine;


namespace Sweet.BuildTools.Editor
{
    [CustomPropertyDrawer(typeof(CompressionModePropertyAttribute))]
    public sealed class CompressionModePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string[] modes = CompressionMode.ModeNames;
            string currentMode = property.stringValue;
            int currentIndex = Array.IndexOf(modes, currentMode);

            if (currentIndex == -1)
            {
                currentMode = "None";
                currentIndex = Array.IndexOf(modes, currentMode);
            }

            currentIndex = EditorGUI.Popup(position, "Compression Mode", currentIndex, modes);
            property.stringValue = modes[currentIndex];
        }
    }
}