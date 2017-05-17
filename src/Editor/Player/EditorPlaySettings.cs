using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;


namespace SweetEditor.Build
{
    [CreateAssetMenu(menuName = "Build Settings/Editor Play", order = 10)]
    public class EditorPlaySettings : ScriptableObject, IBuildSettings
    {
        [SerializeField] private string m_Id = default(string);
        [SerializeField] private SceneAsset m_RunScene = default(SceneAsset);
        [SerializeField] private string m_Defines = default(string);
        [SerializeField] private UnityEvent m_PreRunEvent = default(UnityEvent);




        public string Id
        {
            get { return m_Id; }
        }



        [ContextMenu("Play")]
        public void Run()
        {
            EditorPrefs.SetString(BuildUtility.GetLastSettingsKey<EditorPlaySettings>(), m_Id);

            if (m_RunScene == null)
            {
                Debug.LogWarning(string.Format("No run scene linked in settings {0}.", name), this);
                return;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                m_Defines);

            m_PreRunEvent.Invoke();
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(m_RunScene));
            EditorApplication.isPlaying = true;
        }


        [MenuItem("Edit/Play Last", priority = 151)]
        public static void RunWithLastSettings()
        {
            BuildUtility.GetLastBuildSettings<EditorPlaySettings>().Run();
        }
    }
}
