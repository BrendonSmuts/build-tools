using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


namespace SweetEditor.Build
{
	[CreateAssetMenu(menuName = "Build Settings/Asset Bundle", order = 10)]
	public class AssetBundleBuildSettings : ScriptableObject, IBuildSettings
	{
	    [SerializeField] private string m_Id = default(string);
		[SerializeField] private string m_OutputPath = default(string);
	    [SerializeField] private string[] m_OutputExclusionFilters = default(string[]);
		[SerializeField] private bool m_StrictMode = default(bool);
		[SerializeField] private BuildTarget m_BuildTarget = default(BuildTarget);
		[SerializeField] private string[] m_BuildInclusionFilters = default(string[]);
		[SerializeField, CompressionModeProperty] private string m_CompressionMode = default(string);
		[Header("Events")]
		[SerializeField] private UnityEvent m_PreBuildEvent = default(UnityEvent);
#if UNITY_5_5_OR_NEWER
	    [SerializeField] private PostBuildEvent m_DryRunBuildEvent = default(PostBuildEvent);
#endif
	    [SerializeField] private PostBuildEvent m_PostBuildEvent = default(PostBuildEvent);




		public string CompressionMode
		{
			get { return m_CompressionMode; }
		}


	    public string Id
	    {
	        get { return m_Id; }
	    }




	    private void Reset()
	    {
	        switch (Application.platform)
	        {
#if UNITY_5_5_OR_NEWER
	            case RuntimePlatform.LinuxEditor:
	                m_BuildTarget = BuildTarget.StandaloneLinuxUniversal;
	                m_OutputPath = "AssetBundles/Linux";
	                break;
#endif
	            case RuntimePlatform.OSXEditor:
                    m_BuildTarget = BuildTarget.StandaloneOSXUniversal;
	                m_OutputPath = "AssetBundles/OSX";
	                break;
	            default:
	                m_BuildTarget = BuildTarget.StandaloneWindows;
	                m_OutputPath = "AssetBundles/Windows";
	                break;
	        }

	        m_Id = m_BuildTarget.ToString().ToLower();
	        m_OutputExclusionFilters = new string[0];
	        m_StrictMode = true;
	        m_BuildInclusionFilters = new [] {"*"};
	        m_CompressionMode = new LZ4CompressionMode().Name;
	        m_PreBuildEvent = new Button.ButtonClickedEvent();
#if UNITY_5_5_OR_NEWER
	        m_DryRunBuildEvent = new PostBuildEvent();
#endif
	        m_PostBuildEvent = new PostBuildEvent();
	    }


		[ContextMenu("Run")]
		public void Run()
		{
		    string buildSettingsName = name;
			m_PreBuildEvent.Invoke();

			AssetDatabase.Refresh();
			List<AssetBundleBuild> builds = GetBuildList();

			if (builds.Count == 0)
			{
				Debug.Log(string.Format("AssetBundleBuildSettings ({0}): No valid AssetBundles found in build list.", buildSettingsName), this);
				m_PostBuildEvent.Invoke(new AssetBundleManifest());
				return;
			}

			StringBuilder sb = new StringBuilder(1000);
			sb.AppendLine(string.Format("AssetBundleBuildSettings ({0}): Building...", buildSettingsName));
			for (int i = 0; i < builds.Count; i++)
			{
				var assetBundleBuild = builds[i];

				sb.AppendLine(string.Format("\tName: {0}, Variant: {1}, Asset Count: {2}",
					assetBundleBuild.assetBundleName,
					assetBundleBuild.assetBundleVariant,
					assetBundleBuild.assetNames.Length));
			}
			Debug.Log(sb.ToString(), this);

			BuildAssetBundleOptions options = BuildAssetBundleOptions.None;

		    Build.CompressionMode.GetCompressionMode(m_CompressionMode).OnModifyBuildOptions(this, ref options);

			if (m_StrictMode)
			{
				options |= BuildAssetBundleOptions.StrictMode;
			}

			if (!Directory.Exists(m_OutputPath))
			{
				Directory.CreateDirectory(m_OutputPath);
			}

#if UNITY_5_5_OR_NEWER
			BuildAssetBundleOptions dryOptions = options | BuildAssetBundleOptions.DryRunBuild;
			AssetBundleManifest dryManifest = BuildPipeline.BuildAssetBundles(m_OutputPath, builds.ToArray(), dryOptions, m_BuildTarget);

			if (dryManifest == null)
			{
				string error = string.Format("AssetBundleBuildSettings ({0}): Error during dry run building asset bundles.", buildSettingsName);
				Debug.LogError(error, this);
				throw new Exception(error);
			}

		    m_DryRunBuildEvent.Invoke(dryManifest);
#endif
			bool[] preBuildFileExisted = new bool[builds.Count];
			uint[] preBuildCRCs = new uint[builds.Count];

			for (int i = 0; i < builds.Count; i++)
			{
				AssetBundleBuild build = builds[i];
				string fileIn = GetBundlePath(build);
				preBuildFileExisted[i] = BuildPipeline.GetCRCForAssetBundle(fileIn, out preBuildCRCs[i]);
			}

			AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(m_OutputPath, builds.ToArray(), options, m_BuildTarget);

			if (manifest == null)
			{
				string error = string.Format("AssetBundleBuildSettings ({0}): Error building asset bundles.", buildSettingsName);
				Debug.LogError(error, this);
				throw new Exception(error);
			}

			var newBuilds = new List<AssetBundleBuild>(builds.Count);

			sb.Length = 0;
			sb.AppendLine(string.Format("AssetBundleBuildSettings ({0}): Created/Updated...", buildSettingsName));
			for (int i = 0; i < builds.Count; i++)
			{
				AssetBundleBuild build = builds[i];
				string fileIn = GetBundlePath(build);

				uint crc;
				BuildPipeline.GetCRCForAssetBundle(fileIn, out crc);

				if (!preBuildFileExisted[i] ||
					preBuildCRCs[i] != crc)
				{
					if (preBuildFileExisted[i])
					{
						sb.AppendLine(string.Format("\tUPDATED - Name: {0}, Variant: {1}, Old CRC: {2}, New CRC: {3}",
							build.assetBundleName,
							build.assetBundleVariant,
							preBuildCRCs[i],
							crc));
					}
					else
					{
						sb.AppendLine(string.Format("\tCREATED - Name: {0}, Variant: {1}",
							build.assetBundleName,
							build.assetBundleVariant));
					}

					newBuilds.Add(build);
				}
			}

			if (newBuilds.Count == 0)
			{
				Debug.Log(string.Format("AssetBundleBuildSettings ({0}): Bundles are up to date!", buildSettingsName), this);
			}
			else
			{
				Debug.Log(sb.ToString(), this);
			}


		    Build.CompressionMode.GetCompressionMode(m_CompressionMode).CompressBundles(this, manifest);

			m_PostBuildEvent.Invoke(manifest);
		}


		public void RunAndCopy(string path)
		{
			Run();

			List<AssetBundleBuild> builds = GetOutputList();
			string outputPath = Application.dataPath + "/" + path;

		    if (!outputPath.EndsWith("/"))
		    {
		        outputPath += "/";
		    }

		    if (Directory.Exists(outputPath))
			{
				Directory.Delete(outputPath, true);
			}

		    for (int i = 0; i < builds.Count; i++)
			{
				AssetBundleBuild build = builds[i];

				string bundle = build.assetBundleName;

				if (!string.IsNullOrEmpty(build.assetBundleVariant))
				{
					bundle += "." + build.assetBundleVariant;
				}

			    Build.CompressionMode.GetCompressionMode(m_CompressionMode).ModifyBundleExtension(ref bundle);

				string bundlePath = GetBundlePath(bundle);
			    string destinationPath = outputPath + bundle;
			    string destinationFolder = Path.GetDirectoryName(destinationPath);

			    if (!Directory.Exists(destinationFolder))
			    {
			        Directory.CreateDirectory(destinationFolder);
			    }

			    File.Copy(bundlePath, destinationPath, true);
			}

		    string manfiestFileName = GetManifestFileName();
		    File.Copy(GetBundlePath(manfiestFileName), outputPath + buildTargetName, true);
		    AssetDatabase.Refresh();
		}




		public List<AssetBundleBuild> GetBuildList()
		{
			return GetBuildListWithPredicate(v =>
			        m_BuildInclusionFilters.Length != 0 && m_BuildInclusionFilters.Any(v.Contains));
		}


		public List<AssetBundleBuild> GetOutputList()
		{
		    return GetBuildListWithPredicate(v =>
		        m_BuildInclusionFilters.Length != 0 && m_BuildInclusionFilters.Any(v.Contains) &&
                (m_OutputExclusionFilters.Length == 0 || !m_OutputExclusionFilters.Any(v.Contains)));
		}


		private static List<AssetBundleBuild> GetBuildListWithPredicate(Func<string, bool> buildNamePredicate)
		{
			var builds = new List<AssetBundleBuild>();
			string[] bundleNames = GetBundleNames();

			for (int i = 0; i < bundleNames.Length; i++)
			{
			    string bundleName = bundleNames[i];
			    string[] bundleVariants = GetVariantsForBundleName(bundleName);

			    if (bundleVariants.Length == 0)
			    {
			        string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);

			        builds.Add(new AssetBundleBuild
			        {
			            assetBundleName = bundleName,
			            assetNames = assets
			        });
			    }
			    else
			    {
			        string[] filteredVariants = GetVariantsForBundleName(bundleName).Where(buildNamePredicate).ToArray();

			        for (int j = 0; j < filteredVariants.Length; j++)
			        {
			            string variant = filteredVariants[j];
			            string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName + "." + variant);

			            if (assets.Length == 0)
			            {
			                continue;
			            }

			            builds.Add(new AssetBundleBuild
			            {
			                assetBundleName = bundleName,
			                assetBundleVariant = variant,
			                assetNames = assets
			            });
			        }
			    }
			}

			return builds;
		}


		private string GetManifestFileName()
		{
		    string[] splitPath = Path.GetFullPath(m_OutputPath).Split(Path.DirectorySeparatorChar);

		    if (splitPath.Length == 0)
		    {
		        throw new InvalidOperationException(string.Format("The output path \"{0}\" is not valid.", m_OutputPath));
		    }

		    return splitPath[splitPath.Length - 1];
		}

		//TODO: Filter these??
		public static string[] GetBundleNames()
		{
			return (string[])typeof(AssetDatabase).GetMethod("GetAllAssetBundleNamesWithoutVariant", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
		}


		private static string[] GetVariants()
		{
			return (string[])typeof(AssetDatabase).GetMethod("GetAllAssetBundleVariants", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
		}


		private static string[] GetBundleNamesUsingVariant(string varient)
		{
			varient = varient.ToLowerInvariant();
			if (Array.IndexOf(GetVariants(), varient) == -1)
			{
				return new string[0];
			}

			string[] fullBundleNames = AssetDatabase.GetAllAssetBundleNames();
			return fullBundleNames
				.Where(b => b.EndsWith(varient))
				.Select(b => b.Substring(0, b.Length - varient.Length + 1))
				.ToArray();
		}


		public static string[] GetVariantsForBundleName(string bundleName)
		{
			bundleName = bundleName.ToLowerInvariant();
			if (Array.IndexOf(GetBundleNames(), bundleName) == -1)
			{
				return new string[0];
			}

			string[] fullBundleNames = AssetDatabase.GetAllAssetBundleNames();
			return fullBundleNames
				.Where(b => b.StartsWith(bundleName) && b.Length > bundleName.Length)
				.Select(b => b.Substring(bundleName.Length + 1, b.Length - bundleName.Length - 1))
				.ToArray();
		}


		public string GetBundlePath(AssetBundleBuild build)
		{
			string bundle = GetBundle(build);
			return GetBundlePath(bundle);
		}


		public string GetBundlePath(string bundle)
		{
			string path = Path.GetFullPath(m_OutputPath);
			path += "/" + bundle;

			return path;
		}


		public string GetBundle(AssetBundleBuild build)
		{
			string bundle = build.assetBundleName;

			if (!string.IsNullOrEmpty(build.assetBundleVariant))
			{
				bundle += "." + build.assetBundleVariant;
			}

			return bundle;
		}


		public string[] GetBundleWithVariants()
		{
			return new string[0];
		}




		[Serializable]
		private class PostBuildEvent : UnityEvent<AssetBundleManifest>
		{
		}
	}
}
