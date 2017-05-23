using UnityEditor;
using UnityEngine;

namespace SweetEditor.Build
{
    [CustomEditor(typeof(EditorPlaySettings), true)]
    public sealed class EditorPlaySettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(15);

            if (GUILayout.Button("Play"))
            {
                ((EditorPlaySettings)target).Run();
            }
        }
    }
}