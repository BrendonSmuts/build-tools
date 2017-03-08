#if UNITY_5_5_OR_NEWER || UNITY_5_4_4
#define AUTOMATIC_SIGNING
#endif

using System;
using System.Collections.Generic;
using System.IO;
using SweetEngine.Info;
using UnityEditor;
using UnityEditor.iOS;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using BuildPipeline = UnityEditor.iOS.BuildPipeline;


namespace SweetEditor.Build
{
    [CreateAssetMenu(menuName = "Build Settings/Player/iOS", order = -9)]
    public sealed class iOSPlayerBuildSettings : PlayerBuildSettings
    {
        private static readonly string _DefaultiOSAutomaticSignTeamId = "DefaultiOSAutomaticSignTeamId";
        [Header("iOS")]
        [SerializeField]
        private string m_BundleIdentifier = default(string);
        [SerializeField] private ScriptCallOptimizationLevel m_ScriptCallOptimization = default(ScriptCallOptimizationLevel);
        [SerializeField] private bool m_EnableBitcode = default(bool);
        [SerializeField] private bool m_EnableOnDemandResources = default(bool);
        [SerializeField] private bool m_EnableAppSlicing = default(bool);
        //[SerializeField] private DeviceVariantMap[] m_VariantMaps = default(DeviceVariantMap[]);
        //[SerializeField] private string[] m_InitialInstallTags = default(string[]);
        [SerializeField] private bool m_AutomaticSign = default(bool);
        [SerializeField] private string m_AppleDeveloperTeamId = default(string);
        [SerializeField] private string[] m_Frameworks = default(string[]);
        [SerializeField] private FileInfo[] m_Files = default(FileInfo[]);




        public override BuildTarget BuildTarget
        {
            get { return BuildTarget.iOS; }
        }




        protected override void Reset()
        {
            base.Reset();

            m_BundleIdentifier = PlayerSettings.bundleIdentifier;
            m_ScriptCallOptimization = PlayerSettings.iOS.scriptCallOptimization;

#if AUTOMATIC_SIGNING
            m_AppleDeveloperTeamId = PlayerSettings.iOS.appleDeveloperTeamID;
            m_AutomaticSign = PlayerSettings.iOS.appleEnableAutomaticSigning;

            if (string.IsNullOrEmpty(m_AppleDeveloperTeamId))
            {
                m_AppleDeveloperTeamId = EditorPrefs.GetString(_DefaultiOSAutomaticSignTeamId);
            }
#endif

            m_EnableOnDemandResources = PlayerSettings.iOS.useOnDemandResources;
            m_EnableAppSlicing = PlayerSettings.iOS.useOnDemandResources;
        }

        protected override void OnPushPlayerSettings(Dictionary<string, object> settingsCache)
        {
#if !UNITY_CLOUD
            settingsCache["bundleIdentifier"] = PlayerSettings.bundleIdentifier;

            PlayerSettings.bundleIdentifier = m_BundleIdentifier;
#endif

            settingsCache["scriptCallOptimization"] = PlayerSettings.iOS.scriptCallOptimization;
            settingsCache["useOnDemandResources"] = PlayerSettings.iOS.useOnDemandResources;
#if AUTOMATIC_SIGNING
            settingsCache["appleDeveloperTeamID"] = PlayerSettings.iOS.appleDeveloperTeamID;
            settingsCache["appleEnableAutomaticSigning"] = PlayerSettings.iOS.appleEnableAutomaticSigning;
#endif

            bool useOnDemandResources = m_EnableOnDemandResources || m_EnableAppSlicing;

            PlayerSettings.iOS.scriptCallOptimization = m_ScriptCallOptimization;
            PlayerSettings.iOS.useOnDemandResources = useOnDemandResources;
#if UNITY_5_5_OR_NEWER || UNITY_5_4_4
            PlayerSettings.iOS.appleDeveloperTeamID = m_AppleDeveloperTeamId;
            PlayerSettings.iOS.appleEnableAutomaticSigning = m_AutomaticSign;
#endif

            if (useOnDemandResources)
            {
                //BuildPipeline.collectResources += OnBuildPipelineCollectResources;
                BuildPipeline.collectInitialInstallTags += OnBuildPipelineCollectInitialInstallTags;
            }
        }


        protected override void OnPushManifestPlayerSettings(Dictionary<string, object> settingsCache, BuildManifest manifest)
        {
            settingsCache["buildNumber"] = PlayerSettings.iOS.buildNumber;

            PlayerSettings.iOS.buildNumber = manifest.Build.ToString();
        }


        protected override void OnPopPlayerSettings(Dictionary<string, object> settingsCache)
        {
#if !UNITY_CLOUD
            TrySetValue<string>((v) => PlayerSettings.bundleIdentifier = v, "bundleIdentifier", settingsCache);
#endif
            TrySetValue<ScriptCallOptimizationLevel>((v) => PlayerSettings.iOS.scriptCallOptimization = v, "scriptCallOptimization", settingsCache);
            TrySetValue<bool>((v) => PlayerSettings.iOS.useOnDemandResources = v, "useOnDemandResources", settingsCache);
#if AUTOMATIC_SIGNING
            TrySetValue<string>((v) => PlayerSettings.iOS.appleDeveloperTeamID = v, "appleDeveloperTeamID", settingsCache);
            TrySetValue<bool>((v) => PlayerSettings.iOS.appleEnableAutomaticSigning = v, "appleEnableAutomaticSigning", settingsCache);
#endif

            bool useOnDemandResources = m_EnableOnDemandResources || m_EnableAppSlicing;

            if (useOnDemandResources)
            {
                BuildPipeline.collectResources -= OnBuildPipelineCollectResources;
                BuildPipeline.collectInitialInstallTags -= OnBuildPipelineCollectInitialInstallTags;
            }

            TrySetValue<string>((v) => PlayerSettings.iOS.buildNumber = v, "buildNumber", settingsCache);
        }


        protected override void OnPostProcessBuild(string playerPath)
        {
            string projPath = playerPath + "/Unity-iPhone.xcodeproj/project.pbxproj";

            PBXProject proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));

            string target = proj.TargetGuidByName("Unity-iPhone");

            proj.SetBuildProperty(target, "ENABLE_BITCODE", m_EnableBitcode ? "YES" : "NO");

            for (int i = 0; i < m_Frameworks.Length; i++)
            {
                var s = m_Frameworks[i];
                proj.AddFrameworkToProject(target, s, true);
            }

            for (int i = 0; i < m_Files.Length; i++)
            {
                var s = m_Files[i];
                proj.AddFileToBuild(target, proj.AddFile("usr/lib/" + s.Name, "Frameworks/" + s.Name, (PBXSourceTree)s.SourceTree));
            }

            File.WriteAllText(projPath, proj.WriteToString());
        }


        private Resource[] OnBuildPipelineCollectResources()
        {
            return new Resource[0];
            //if (AssetBundleBuildSettings == null)
            //{
            //	return new Resource[0];
            //}
            //
            //var resources = new List<Resource>();
            //string[] bundles = AssetBundleBuildSettings.GetBundleNames();
            //
            //for (int i = 0; i < bundles.Length; i++)
            //{
            //	string bundle = bundles[i];
            //	Resource resource = new Resource(bundle, AssetBundleBuildSettings.GetBundlePath(bundle))
            //		.AddOnDemandResourceTags(bundle);
            //
            //	string[] varients = AssetBundleBuildSettings.GetVariantsForBundleName(bundle);
            //
            //	for (int j = 0; j < varients.Length; j++)
            //	{
            //		string variant = varients[j];
            //		DeviceVariantMap variantMap = m_VariantMaps.FirstOrDefault(vm => string.Equals(vm.Variant, variant));
            //
            //		if (variantMap == null)
            //		{
            //			string error =
            //				string.Format(
            //					"Missing a device variant mapping for varient \"{0}\". Ensure all bundle variants have valid mappings.", variant);
            //
            //			Debug.LogError(error, this);
            //			throw new InvalidOperationException(error);
            //		}
            //
            //		string bundleVariant = string.Format("{0}.{1}", bundle, variant);
            //
            //		for (int k = 0; k < variantMap.Requirements.Length; k++)
            //		{
            //			iOSDeviceDescription desc = variantMap.Requirements[k];
            //			resource.BindVariant(AssetBundleBuildSettings.GetBundlePath(bundleVariant), (iOSDeviceRequirement)desc);
            //		}
            //	}
            //
            //	resources.Add(resource);
            //}
            //
            //return resources.ToArray();
        }


        private string[] OnBuildPipelineCollectInitialInstallTags()
        {
            return new string[0];

            //if (AssetBundleBuildSettings == null)
            //{
            //	return new string[0];
            //}
            //
            //string[] bundleNames = AssetBundleBuildSettings.GetBundleNames();
            //
            //for (int i = 0; i < m_InitialInstallTags.Length; i++)
            //{
            //	string initialInstallTag = m_InitialInstallTags[i];
            //
            //	if (Array.IndexOf(bundleNames, initialInstallTag) == -1)
            //	{
            //		string error =
            //			string.Format(
            //				"Install tag \"{0}\" cannot be found in the bundle names supplied by the current AssetBundleSettings.", initialInstallTag);
            //
            //		Debug.LogError(error, this);
            //		throw new InvalidOperationException(error);
            //	}
            //}
            //
            //return m_InitialInstallTags;
        }



        [Serializable]
        private class FileInfo
        {
            public string Name = default(string);
            public SourceTree SourceTree = default(SourceTree);
        }

        [Serializable]
        private class DeviceVariantMap
        {
            public string Variant = default(string);
            public iOSDeviceDescription[] Requirements = default(iOSDeviceDescription[]);
        }


        public enum SourceTree
        {
            Absolute,
            Source,
            Group,
            Build,
            Developer,
            Sdk
        }
    }
}