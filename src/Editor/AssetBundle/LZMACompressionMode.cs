﻿using UnityEditor;
using UnityEngine;


namespace SweetEditor.Build
{
    public sealed class LZMACompressionMode : ICompressionMode
    {
        public string Name
        {
            get
            {
                return "LZMA";

            }
        }





        public void OnModifyBuildOptions(AssetBundleBuildSettings settings, ref BuildAssetBundleOptions options)
        {
        }


        public void CompressBundles(AssetBundleBuildSettings settings, AssetBundleManifest manifest)
        {
        }


        public void ModifyBundleExtension(ref string bundle)
        {

        }
    }
}