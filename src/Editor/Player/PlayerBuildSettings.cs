using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SweetEngine.Info;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;


namespace SweetEditor.Build
{
    /// <summary>
    ///
    /// </summary>
    public abstract class PlayerBuildSettings : ScriptableObject, IBuildSettings
    {
        [SerializeField] private string m_Id = default(string);
        [Header("Player")]
        [SerializeField]
        private string m_ProductName = default(string);
        [SerializeField] private SceneAsset[] m_Scenes = default(SceneAsset[]);
        [SerializeField] private string m_OutputPath = default(string);
        [SerializeField] private bool m_IsDevelopment = default(bool);
        [SerializeField] private string m_Defines = default(string);
        [Header("Events")]
        [SerializeField]
        private UnityEvent m_PreExportEvent = default(UnityEvent);
        [SerializeField] private PostExportEvent m_PostExportEvent = default(PostExportEvent);




        public abstract BuildTarget BuildTarget { get; }


        public string Id
        {
            get { return m_Id; }
        }




        protected virtual void Reset()
        {
            m_Id = "dev-" + GetPlatformName().ToLower();
            m_ProductName = PlayerSettings.productName;
            m_Scenes = EditorBuildSettings.scenes
                .Select<EditorBuildSettingsScene, SceneAsset>(s => AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path))
                .ToArray();
            m_OutputPath = "Builds/DEV/{platform}/{product}_{id}";
            m_IsDevelopment = true;
            m_Defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(GetGroupForBuildTarget(BuildTarget));
        }


        protected string GetPlatformName()
        {
            return BuildTarget.ToString();
        }


        [ContextMenu("Build")]
        public void Run()
        {
            Run(false);
        }

        [ContextMenu("Build and Run")]
        public void RunAndDeploy()
        {
            Run(true);
        }

        [MenuItem("File/Build Last", priority = 0)]
        public static void RunWithLastSettings()
        {
            BuildUtility.GetLastBuildSettings<PlayerBuildSettings>().Run();
        }

        [MenuItem("File/Build and Run Last", priority = 0)]
        public static void RunAndDeployWithLastSettings()
        {
            BuildUtility.GetLastBuildSettings<PlayerBuildSettings>().RunAndDeploy();
        }


        public void Run(bool deploy)
        {
            PlayerPrefs.SetString(BuildUtility.GetLastSettingsKey<PlayerBuildSettings>(), m_Id);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            List<string> scenes = new List<string>(m_Scenes.Length);

            for (int i = 0; i < m_Scenes.Length; i++)
            {
                SceneAsset sceneAsset = m_Scenes[i];

                if (sceneAsset == null)
                {
                    throw new BuildException("Missing scene asset reference in BuildSettings. Has this scene been deleted?");
                }

                scenes.Add(AssetDatabase.GetAssetPath(sceneAsset));
            }

            if (scenes.Count == 0)
            {
                throw new BuildException("No scene asset references in BuildSettings. At least one scene must be added when building a player.");
            }

            BuildOptions buildOptions = BuildOptions.None;

            if (m_IsDevelopment)
            {
                buildOptions |= BuildOptions.Development;
                buildOptions |= BuildOptions.AllowDebugging;
            }

            if (deploy)
            {
                buildOptions |= BuildOptions.AutoRunPlayer;
            }

            var settingsCache = new Dictionary<string, object>();

            try
            {
                PushPlayerSettings(settingsCache);
                PreProcessBuild();

                BuildManifest manifest = CreateOrUpdateBuildManifest();
                string outputPath = GetOutputPath(manifest);
                string buildPath = PrepareBuildPath(outputPath);
                PushManifestPlayerSettings(settingsCache, manifest);

                FileInfo fileInfo = new FileInfo(buildPath);

                string error = BuildPipeline.BuildPlayer(scenes.ToArray(), buildPath, BuildTarget,
                    buildOptions);

                if (!string.IsNullOrEmpty(error))
                {
                    throw new BuildException("Error building player: " + error);
                }

                PostProcessBuild(outputPath);
            }
            finally
            {
                PopPlayerSettings(settingsCache);
            }
        }


        protected virtual string PrepareBuildPath(string outputPath)
        {
            DirectoryInfo di =
                new DirectoryInfo(Application.dataPath.Substring(0, Application.dataPath.Length - 6) + outputPath);

            if (!di.Exists)
            {
                di.Create();
            }

            return outputPath;
        }


        internal void PushPlayerSettings(Dictionary<string, object> settingsCache)
        {
            settingsCache["scripting_define_symbols"] = PlayerSettings.GetScriptingDefineSymbolsForGroup(GetGroupForBuildTarget(BuildTarget));
            settingsCache["product_name"] = PlayerSettings.productName;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(GetGroupForBuildTarget(BuildTarget), m_Defines);
            PlayerSettings.productName = m_ProductName;

            OnPushPlayerSettings(settingsCache);
        }


        private void PushManifestPlayerSettings(Dictionary<string, object> settingsCache, BuildManifest manifest)
        {
            settingsCache["bundle_version"] = PlayerSettings.bundleVersion;

            PlayerSettings.bundleVersion = manifest.GetVersionString();

            OnPushManifestPlayerSettings(settingsCache, manifest);
        }

        private void PopPlayerSettings(Dictionary<string, object> settingsCache)
        {
            TrySetValue<string>((v) => PlayerSettings.SetScriptingDefineSymbolsForGroup(GetGroupForBuildTarget(BuildTarget), v), "scripting_define_symbols", settingsCache);
            TrySetValue<string>((v) => PlayerSettings.productName = v, "product_name", settingsCache);
            TrySetValue<string>((v) => PlayerSettings.bundleVersion = v, "bundle_version", settingsCache);

            OnPopPlayerSettings(settingsCache);
        }


        public void PreProcessBuild()
        {
            OnPreProcessBuild();
            m_PreExportEvent.Invoke();
        }


        public void PostProcessBuild(string playerPath)
        {
            OnPostProcessBuild(playerPath);
            m_PostExportEvent.Invoke(playerPath);
        }


        protected virtual void OnPushPlayerSettings(Dictionary<string, object> settingsCache)
        {

        }


        protected virtual void OnPushManifestPlayerSettings(Dictionary<string, object> settingsCache, BuildManifest manifest)
        {

        }


        protected virtual void OnPopPlayerSettings(Dictionary<string, object> settingsCache)
        {

        }


        protected virtual void OnPreProcessBuild()
        {

        }


        protected virtual void OnPostProcessBuild(string playerPath)
        {

        }


        private static BuildTargetGroup GetGroupForBuildTarget(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return BuildTargetGroup.Standalone;
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                case BuildTarget.iOS:
                    return BuildTargetGroup.iOS;
                case BuildTarget.WebGL:
                    return BuildTargetGroup.WebGL;
                default:
                    throw new ArgumentException(string.Format("No conversion defined for build target {0}", target), "target");
            }
        }


        private string GetOutputPath(BuildManifest manifest)
        {
            string ret = m_OutputPath.Replace("{id}", m_Id.ToLower());
            ret = ret.Replace("{product}", m_ProductName.ToLower());
            ret = ret.Replace("{platform}", GetPlatformName().ToLower());
            ret = ret.Replace("{version}", manifest.GetVersionString());
            ret = ret.Replace("{short_version}", manifest.GetShortVersionString());
            ret = ret.Replace("{build}", manifest.Build.ToString());
            ret = ret.Replace("{branch}", manifest.ScmBranch);
            ret = ret.Replace("{commit}", manifest.ScmCommitId);
            return ret;
        }


        private static BuildManifest CreateOrUpdateBuildManifest()
        {
            BuildManifest manifest;
            string manifestPath = "Assets/Resources/" + BuildManifest.ManifestName + ".asset";
            string fullPath = Path.GetFullPath(manifestPath);

            if (!File.Exists(fullPath))
            {
                string directoryPath = Path.GetDirectoryName(fullPath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                manifest = CreateInstance<BuildManifest>();
                AssetDatabase.CreateAsset(manifest, manifestPath);
            }
            else
            {
                manifest = AssetDatabase.LoadAssetAtPath<BuildManifest>(manifestPath);

                if (manifest == null)
                {
                    throw new BuildException(
                        string.Format(
                            "Failed to load build manifest at path \"{0}\", this location is reserved. If the asset at this path is missing a script reference try deletingit.",
                            manifestPath));
                }
            }

            SerializedObject soManifest = new SerializedObject(manifest);

            SerializedProperty spBuild = soManifest.FindProperty("m_Build");
            SerializedProperty spScmCommitId = soManifest.FindProperty("m_ScmCommitId");
            SerializedProperty spScmBranch = soManifest.FindProperty("m_ScmBranch");
            SerializedProperty spBuildTime = soManifest.FindProperty("m_BuildTime");
            SerializedProperty spBundleId = soManifest.FindProperty("m_BundleId");
            SerializedProperty spUnityVersion = soManifest.FindProperty("m_UnityVersion");

            spBuild.intValue++;
            spScmCommitId.stringValue = GetGitCommandOutput("rev-parse HEAD");
            spScmBranch.stringValue = GetGitCommandOutput("rev-parse --abbrev-ref HEAD");
            spBuildTime.stringValue = DateTime.UtcNow.ToString("d MMM yyyy");
            spBundleId.stringValue = PlayerSettings.applicationIdentifier;
            spUnityVersion.stringValue = Application.unityVersion;

            soManifest.ApplyModifiedPropertiesWithoutUndo();

            return manifest;
        }


        protected static void TrySetValue<T>(Action<T> setter, string key,
            Dictionary<string, object> settingsCache)
        {
            object value;

            if (!settingsCache.TryGetValue(key, out value))
            {
                return;
            }

            setter((T)value);
        }


        private static string GetGitCommandOutput(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                CreateNoWindow = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            Process p = new Process
            {
                StartInfo = startInfo
            };

            string output = string.Empty;

            try
            {
                if (!p.Start())
                {
                    Debug.LogError("GetGitCommandOutput: Failed to start process.");
                }
                else
                {
                    string error = p.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError("GetGitCommandOutput ERROR: " + error);
                    }

                    output = p.StandardOutput.ReadToEnd();
                    output = output.Trim('\n');

                    p.WaitForExit();

                    if (p.ExitCode != 0)
                    {
                        Debug.LogError(string.Format("GetGitCommandOutput: Non-zero exit code {0}.", p.ExitCode));
                    }

                }
            }
            catch (Exception e)
            {
                Debug.LogError("GetGitCommandOutput EXCEPTION: " + e);
            }

            return output;
        }



        [Serializable]
        private class PostExportEvent : UnityEvent<string>
        {
        }
    }
}
