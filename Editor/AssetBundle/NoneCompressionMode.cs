using UnityEditor;
using UnityEngine;


namespace Sweet.BuildTools.Editor
{
    public sealed class NoneCompressionMode : ICompressionMode
    {
        public string Name
        {
            get
            {
                return "None";

            }
        }





        public void OnModifyBuildOptions(AssetBundleBuildSettings settings, ref BuildAssetBundleOptions options)
        {
            options |= BuildAssetBundleOptions.UncompressedAssetBundle;
        }


        public void CompressBundles(AssetBundleBuildSettings settings, AssetBundleManifest manifest)
        {
        }


        public void ModifyBundleExtension(ref string bundle)
        {

        }
    }
}