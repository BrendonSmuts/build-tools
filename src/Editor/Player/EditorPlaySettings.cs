using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;


namespace SweetEditor.Build
{
    [CreateAssetMenu(menuName = "Build Settings/Editor Play", order = 10)]
    public class EditorPlaySettings : ScriptableObject
    {
        private static readonly string _LastPlayKey = "lastPlaySettings";
        [SerializeField] private string m_SettingsName = default(string);
        [SerializeField] private SceneAsset m_RunScene = default(SceneAsset);
        [SerializeField] private string m_Defines = default(string);
        [SerializeField] private UnityEvent m_PreRunEvent = default(UnityEvent);



        [ContextMenu("Play")]
        private void Play()
        {
            if (m_RunScene == null)
            {
                Debug.LogWarning(string.Format("No run scene linked in settings {0}.", name), this);
                return;
            }

            EditorPrefs.SetString(_LastPlayKey, m_SettingsName);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                m_Defines);

            m_PreRunEvent.Invoke();

            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(m_RunScene));
            EditorApplication.isPlaying = true;
        }



        [MenuItem("Edit/Play with Settings", priority = 150)]
        private static void PlayWithSettings()
        {
            GenericMenu settingsMenu = new GenericMenu();

            EditorPlaySettings[] settings = AssetDatabase.FindAssets("t:EditorPlaySettings")
                .Select<string, string>(AssetDatabase.GUIDToAssetPath)
                .Select<string, EditorPlaySettings>(AssetDatabase.LoadAssetAtPath<EditorPlaySettings>)
                .ToArray();

            if (settings.Length == 0)
            {
                settingsMenu.AddItem(new GUIContent("No Play Settings Found"), false, () => Debug.Log("Pressed"));
            }
            else
            {
                for (int i = 0; i < settings.Length; i++)
                {
                    EditorPlaySettings s = settings[i];
                    settingsMenu.AddItem(new GUIContent(s.m_SettingsName), false, OnMenuItemClicked, s);
                }
            }

            settingsMenu.ShowAsContext();
        }


        [MenuItem("Edit/Play with Last Settings", priority = 151)]
        private static void PlayWithLastSettings()
        {
            string lastPlaySettings = EditorPrefs.GetString(_LastPlayKey, string.Empty);

            EditorPlaySettings s = AssetDatabase.FindAssets("t:EditorPlaySettings")
                .Select<string, string>(AssetDatabase.GUIDToAssetPath)
                .Select<string, EditorPlaySettings>(AssetDatabase.LoadAssetAtPath<EditorPlaySettings>)
                .FirstOrDefault(eps => string.Equals(eps.m_SettingsName, lastPlaySettings));

            if (s != null)
            {
                s.Play();
            }
        }

        private static void OnMenuItemClicked(object userdata)
        {
            ((EditorPlaySettings)userdata).Play();
        }
    }
}
