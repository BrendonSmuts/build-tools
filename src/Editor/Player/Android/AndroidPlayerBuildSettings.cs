using System;
using System.Collections.Generic;
using System.IO;
using SweetEngine.Info;
using UnityEditor;
using UnityEngine;


namespace SweetEditor.Build
{
	[CreateAssetMenu(menuName = "Build Settings/Player/Android", order = -8)]
	public sealed class AndroidPlayerBuildSettings : PlayerBuildSettings
	{
		[Header("Android")]
		[SerializeField] private string m_BundleIdentifier = default(string);
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

	        m_BundleIdentifier = PlayerSettings.bundleIdentifier;
	        m_Keystore = PlayerSettings.Android.keystoreName;
	        m_KeystorePassword = PlayerSettings.Android.keystorePass;
	        m_KeyAlias = PlayerSettings.Android.keyaliasName;
	        m_KeyAliasPassword = PlayerSettings.Android.keyaliasPass;
	        m_SplitApplicationBinary = PlayerSettings.Android.useAPKExpansionFiles;
	    }


	    protected override string PrepareBuildPath(string outputPath)
		{
			outputPath += ".apk";
			
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
			settingsCache["bundleIdentifier"] = PlayerSettings.bundleIdentifier;
			settingsCache["keystoreName"] = PlayerSettings.Android.keystoreName;
			settingsCache["keystorePass"] = PlayerSettings.Android.keystorePass;
			settingsCache["keyaliasName"] = PlayerSettings.Android.keyaliasName;
			settingsCache["keyaliasPass"] = PlayerSettings.Android.keyaliasPass;
			settingsCache["useAPKExpansionFiles"] = PlayerSettings.Android.useAPKExpansionFiles;

			PlayerSettings.bundleIdentifier = m_BundleIdentifier;
			PlayerSettings.Android.keystoreName = m_Keystore;
			PlayerSettings.Android.keystorePass = m_KeystorePassword;
			PlayerSettings.Android.keyaliasName = m_KeyAlias;
			PlayerSettings.Android.keyaliasPass = m_KeyAliasPassword;
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
		    TrySetValue<string>((v) => PlayerSettings.bundleIdentifier = v, "bundleIdentifier", settingsCache);
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