using UnityEditor;
using UnityEngine;


namespace Sweet.BuildTools.Editor
{
    public sealed class LZ4CompressionMode : ICompressionMode
    {
        public string Name
        {
            get
            {
                return "LZ4 (Chunk)";

            }
        }





        public void OnModifyBuildOptions(AssetBundleBuildSettings settings, ref BuildAssetBundleOptions options)
        {
            options |= BuildAssetBundleOptions.ChunkBasedCompression;
        }


        public void CompressBundles(AssetBundleBuildSettings settings, AssetBundleManifest manifest)
        {
        }


        public void ModifyBundleExtension(ref string bundle)
        {
        }
    }
}