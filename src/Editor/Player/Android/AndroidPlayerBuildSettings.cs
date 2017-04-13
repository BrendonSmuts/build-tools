using System;
using System.Collections.Generic;
using System.IO;
using SweetEngine.Build;
using UnityEditor;
using UnityEngine;


namespace SweetEditor.Build
{
    [CreateAssetMenu(menuName = "Build Settings/Player/Android", order = -8)]
    public sealed class AndroidPlayerBuildSettings : PlayerBuildSettings
    {
        [Header("Android")]
        [SerializeField]
        private string m_BundleIdentifier = default(string);
        [SerializeField] private string m_Keystore = default(string);
        [SerializeField] private string m_KeystorePassword = default(string);
        [SerializeField] private string m_KeyAlias = default(string);
        [SerializeField] private string m_KeyAliasPassword = default(string);
        [SerializeField] private bool m_SplitApplicationBinary = default(bool);
        [SerializeField] private int m_PlayStoreDeliveryPriority = default(int);
        [SerializeField] private AndroidDeviceDescription m_DeviceRequirement = default(AndroidDeviceDescription);




        public override BuildTarget BuildTarget
        {
            get { return BuildTarget.Android; }
        }




        protected override void Reset()
        {
            base.Reset();

#if UNITY_5_6_OR_NEWER
            m_BundleIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
#else
            m_BundleIdentifier = PlayerSettings.bundleIdentifier;
#endif

            m_Keystore = PlayerSettings.Android.keystoreName;
            m_KeystorePassword = PlayerSettings.Android.keystorePass;
            m_KeyAlias = PlayerSettings.Android.keyaliasName;
            m_KeyAliasPassword = PlayerSettings.Android.keyaliasPass;
            m_SplitApplicationBinary = PlayerSettings.Android.useAPKExpansionFiles;
        }


        protected override string PrepareBuildPath(string outputPath)
        {
            if (!outputPath.EndsWith(".apk"))
            {
                outputPath += ".apk";
            }

            FileInfo fi = new FileInfo(outputPath);

            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            if (fi.Exists)
            {
                fi.Delete();
            }

            return outputPath;
        }


        protected override void OnPushPlayerSettings(Dictionary<string, object> settingsCache)
        {
#if !UNITY_CLOUD
#if UNITY_5_6_OR_NEWER
            settingsCache["bundleIdentifier"] = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, m_BundleIdentifier);
#else
            settingsCache["bundleIdentifier"] = PlayerSettings.bundleIdentifier;
            PlayerSettings.bundleIdentifier = m_BundleIdentifier;
#endif
            settingsCache["keystoreName"] = PlayerSettings.Android.keystoreName;
            PlayerSettings.Android.keystoreName = m_Keystore;

            settingsCache["keystorePass"] = PlayerSettings.Android.keystorePass;
            PlayerSettings.Android.keystorePass = m_KeystorePassword;

            settingsCache["keyaliasName"] = PlayerSettings.Android.keyaliasName;
            PlayerSettings.Android.keyaliasName = m_KeyAlias;

            settingsCache["keyaliasPass"] = PlayerSettings.Android.keyaliasPass;
            PlayerSettings.Android.keyaliasPass = m_KeyAliasPassword;

            settingsCache["useAPKExpansionFiles"] = PlayerSettings.Android.useAPKExpansionFiles;
            PlayerSettings.Android.useAPKExpansionFiles = m_SplitApplicationBinary;
#endif

            settingsCache["androidBuildSubtarget"] = EditorUserBuildSettings.androidBuildSubtarget;
            EditorUserBuildSettings.androidBuildSubtarget = m_DeviceRequirement.TextureCompression;
        }


        protected override void OnPushManifestPlayerSettings(Dictionary<string, object> settingsCache,
            BuildManifest manifest)
        {
            settingsCache["bundleVersionCode"] = PlayerSettings.Android.bundleVersionCode;

            PlayerSettings.Android.bundleVersionCode = manifest.Build + m_PlayStoreDeliveryPriority;
        }


        protected override void OnPopPlayerSettings(Dictionary<string, object> settingsCache)
        {
#if !UNITY_CLOUD
#if UNITY_5_6_OR_NEWER
            TrySetValue<string>((v) => PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, v), "bundleIdentifier", settingsCache);
#else
            TrySetValue<string>((v) => PlayerSettings.bundleIdentifier = v, "bundleIdentifier", settingsCache);
#endif
            TrySetValue<string>((v) => PlayerSettings.applicationIdentifier = v, "bundleIdentifier", settingsCache);
            TrySetValue<string>((v) => PlayerSettings.Android.keystoreName = v, "keystoreName", settingsCache);
            TrySetValue<string>((v) => PlayerSettings.Android.keystorePass = v, "keystorePass", settingsCache);
            TrySetValue<string>((v) => PlayerSettings.Android.keyaliasName = v, "keyaliasName", settingsCache);
            TrySetValue<string>((v) => PlayerSettings.Android.keyaliasPass = v, "keyaliasPass", settingsCache);
            TrySetValue<bool>((v) => PlayerSettings.Android.useAPKExpansionFiles = v, "useAPKExpansionFiles", settingsCache);
#endif
            TrySetValue<MobileTextureSubtarget>((v) => EditorUserBuildSettings.androidBuildSubtarget = v, "androidBuildSubtarget", settingsCache);
            TrySetValue<int>((v) => PlayerSettings.Android.bundleVersionCode = v, "bundleVersionCode", settingsCache);
        }



        [Serializable]
        private class DeviceVariantMap
        {
            public string Variant = default(string);
            public AndroidDeviceDescription Requirement = default(AndroidDeviceDescription);
        }
    }
}