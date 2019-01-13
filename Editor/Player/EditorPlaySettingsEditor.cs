using UnityEditor;
using UnityEngine;

namespace Sweet.BuildTools.Editor
{
    [CustomEditor(typeof(EditorPlaySettings), true)]
    public sealed class EditorPlaySettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(15);

            if (GUILayout.Button("Play"))
            {
                ((EditorPlaySettings)target).Run();
                return;
            }
        }
    }
}