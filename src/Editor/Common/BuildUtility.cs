using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;


namespace SweetEditor.Build
{
    public static class BuildUtility
    {
        private static readonly string _LastPlayKey = "lastPlaySettings";




        /// <summary>
        /// Entry point for running build settings through Unity batch mode.
        /// </summary> 
        /// <example>
        /// path/to/unity.exe \
        ///     -quit \
        ///     -batchmode \
        ///     -nographics \
        ///     -projectPath '/path/to/project/' \
        ///     -executeMethod SweetEditor.Build.BuildUtility.RunPlayerBuildSettingsCmdLine -buildSettings standalonewindows
        /// </example>
        [UsedImplicitly]
        private static void RunPlayerBuildSettingsCmdLine()
        {
            string[] args = Environment.CommandLine.Split(' ');

            int buildSettingsIndex = Array.IndexOf(args, "-buildSettings");
            if (buildSettingsIndex == -1)
            {
                throw new Exception(
                    "Missing build settings id argument. When executing RunPlayerBuildSettingsCmdLine include a build settings argument with a player build settings Id. e.g. \"-buildSettings dev-standalonewindows\".");
            }

            int androidSdkPathIndex = Array.IndexOf(args, "-androidSdkPath");
            if (androidSdkPathIndex != -1)
            {
                Debug.Log("Setting AndroidSdkRoot: " + args[androidSdkPathIndex + 1]);
                EditorPrefs.SetString("AndroidSdkRoot", args[androidSdkPathIndex + 1]);
            }

            int androidNdkPathIndex = Array.IndexOf(args, "-androidNdkPath");
            if (androidNdkPathIndex != -1)
            {
                Debug.Log("Setting AndroidNdkRoot: " + args[androidNdkPathIndex + 1]);
                EditorPrefs.SetString("AndroidNdkRoot", args[androidNdkPathIndex + 1]);
            }

            RunPlayerBuildSettings(args[buildSettingsIndex + 1]);
        }


        [UsedImplicitly]
        private static void RunAssetBundleBuildSettingsCmdLine()
        {
            string[] args = Environment.CommandLine.Split(' ');

            int buildSettingsIndex = Array.IndexOf(args, "-buildSettings");
            if (buildSettingsIndex == -1)
            {
                throw new Exception(
                    "Missing build settings id argument. When executing RunAssetbundleBuildSettingsCmdLine include a build settings argument with an asset bundle build settings Id. e.g. \"-buildSettings dev-standalonewindows\".");
            }

            RunPlayerBuildSettings(args[buildSettingsIndex + 1]);
        }


        public static void RunPlayerBuildSettingsPreExport(string id)
        {
            var settingsCache = new Dictionary<string, object>();
            var buildSettings = FindBuildSettings<PlayerBuildSettings>(id);
            buildSettings.PushPlayerSettings(settingsCache);
            buildSettings.PreProcessBuild();
        }


        public static void RunPlayerBuildSettingsPostExport(string id, string playerPath)
        {
            FindBuildSettings<PlayerBuildSettings>(id).PostProcessBuild(playerPath);
        }


        public static void RunPlayerBuildSettings(string id)
        {
            FindBuildSettings<PlayerBuildSettings>(id).Run();
        }


        public static void RunAssetBundleBuildSettings(string id)
        {
            FindBuildSettings<AssetBundleBuildSettings>(id).Run();
        }


        public static TBuildSettings FindBuildSettings<TBuildSettings>(string id)
            where TBuildSettings : ScriptableObject, IBuildSettings
        {
            return FindBuildSettings<TBuildSettings>(id, false);
        }

        public static TBuildSettings FindBuildSettings<TBuildSettings>(string id, bool throwOnMissing)
            where TBuildSettings : ScriptableObject, IBuildSettings
        {
            TBuildSettings buildSettings = AssetDatabase.FindAssets("t:" + typeof(TBuildSettings).Name)
                .Select<string, string>(AssetDatabase.GUIDToAssetPath)
                .Select<string, TBuildSettings>(AssetDatabase.LoadAssetAtPath<TBuildSettings>)
                .FirstOrDefault(abbs => string.Equals(abbs.Id, id, StringComparison.OrdinalIgnoreCase));

            if (throwOnMissing &&
                buildSettings == null)
            {
                throw new MissingBuildSettingsException(typeof(TBuildSettings), id);
            }

            return buildSettings;
        }


        public static TBuildSettings GetLastBuildSettings<TBuildSettings>()
            where TBuildSettings : ScriptableObject, IBuildSettings
        {
            return FindBuildSettings<TBuildSettings>(PlayerPrefs.GetString(GetLastSettingsKey<TBuildSettings>(), string.Empty), false);
        }

        public static string GetLastSettingsKey<TBuildSettings>()
        {
            return string.Format("{0}.{1}", typeof(TBuildSettings).Name, _LastPlayKey);
        }
    }
}