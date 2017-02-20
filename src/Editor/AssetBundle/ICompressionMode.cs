using UnityEditor;
using UnityEngine;


namespace SweetEditor.Build
{
    public interface ICompressionMode
    {
        string Name { get; }


        void OnModifyBuildOptions(AssetBundleBuildSettings settings, ref BuildAssetBundleOptions options);


        void CompressBundles(AssetBundleBuildSettings settings, AssetBundleManifest manifest);


        void ModifyBundleExtension(ref string bundle);
    }
}