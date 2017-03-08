using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace SweetEditor.Build
{
    public class UBuildEditorWindow : EditorWindow
    {
        private static GUILayoutOption[] _SettingsButtonLayoutOptions = new [] {
            GUILayout.Width(90)
        };
        private static GUILayoutOption[] _SelectButtonLayoutOptions = new [] {
            GUILayout.Width(20)
        };
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
        }


        /// <summary>
        /// OnGUI is called for rendering and handling GUI events.
        /// This function can be called multiple times per frame (one call per event).
        /// </summary>
        private void OnGUI()
        {
            PlayerBuildSettings lastPlayerBuildSettings = BuildUtility.GetLastBuildSettings<PlayerBuildSettings>();
            EditorPlaySettings lastEditorPlaySettings = BuildUtility.GetLastBuildSettings<EditorPlaySettings>();
            string lastPlayerName = lastPlayerBuildSettings.name;
            string lastEditorName = lastEditorPlaySettings.name;

            if (lastPlayerBuildSettings != null)
            {
                if (GUILayout.Button("Build - " + lastPlayerName))
                {
                    lastPlayerBuildSettings.Run();
                    return;
                }

                if (GUILayout.Button("Build and Run - " + lastPlayerName))
                {
                    lastPlayerBuildSettings.RunAndDeploy();
                    return;
                }
            }
            else
            {

            }

            if (lastEditorPlaySettings != null)
            {
                if (GUILayout.Button("Play - " + lastEditorName))
                {
                    lastEditorPlaySettings.Run();
                    return;
                }
            }
            else
            {

            }

            PlayerBuildSettings[] playerBuildSettings = AssetDatabase.FindAssets("t:PlayerBuildSettings")
                    .Select<string, string>(AssetDatabase.GUIDToAssetPath)
                    .Select<string, PlayerBuildSettings>(AssetDatabase.LoadAssetAtPath<PlayerBuildSettings>)
                    .ToArray();

            if (playerBuildSettings.Length == 0)
            {
                EditorGUILayout.TextArea("No editor play settings found. To create a settings object right click a location in the Project window and select Create->Build Settings->Editor Play");
            }
            else
            {
                GUILayout.Label("Players");

                for (int i = 0; i < playerBuildSettings.Length; i++)
                {
                    PlayerBuildSettings setting = playerBuildSettings[i];

                    GUILayout.BeginHorizontal();

                    GUILayout.Label(setting.name);
                    if (GUILayout.Button("Build", _SettingsButtonLayoutOptions))
                    {
                        setting.Run();
                    }

                    if (GUILayout.Button("Build and Run", _SettingsButtonLayoutOptions))
                    {
                        setting.RunAndDeploy();
                    }

                    if (GUILayout.Button("?", _SelectButtonLayoutOptions))
                    {
                        Selection.activeObject = setting;
                    }

                    GUILayout.EndHorizontal();
                }
            }


            EditorPlaySettings[] editorPlaySettings = AssetDatabase.FindAssets("t:EditorPlaySettings")
                    .Select<string, string>(AssetDatabase.GUIDToAssetPath)
                    .Select<string, EditorPlaySettings>(AssetDatabase.LoadAssetAtPath<EditorPlaySettings>)
                    .ToArray();

            if (editorPlaySettings.Length == 0)
            {
                EditorGUILayout.TextArea("No editor play settings found. To create a settings object right click a location in the Project window and select Create->Build Settings->Editor Play");
            }
            else
            {
                GUILayout.Label("Editor Play");

                for (int i = 0; i < editorPlaySettings.Length; i++)
                {
                    EditorPlaySettings setting = editorPlaySettings[i];

                    GUILayout.BeginHorizontal();

                    GUILayout.Label(setting.name);
                    if (GUILayout.Button("Play", _SettingsButtonLayoutOptions))
                    {
                        setting.Run();
                    }

                    if (GUILayout.Button("?", _SelectButtonLayoutOptions))
                    {
                        Selection.activeObject = setting;
                    }

                    GUILayout.EndHorizontal();
                }
            }
        }

        [MenuItem("Window/uBuild")]
        public static void ShowWindow() 
        {
            EditorWindow.GetWindow<UBuildEditorWindow>("uBuild", true);
        }
    }
}