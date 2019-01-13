using UnityEditor;
using UnityEngine;


namespace Sweet.BuildTools.Editor
{
    public interface ICompressionMode
    {
        string Name { get; }


        void OnModifyBuildOptions(AssetBundleBuildSettings settings, ref BuildAssetBundleOptions options);


        void CompressBundles(AssetBundleBuildSettings settings, AssetBundleManifest manifest);


        void ModifyBundleExtension(ref string bundle);
    }
}