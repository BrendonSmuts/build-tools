using System;
using UnityEditor;
using UnityEngine;

namespace Sweet.BuildTools.Editor
{
    [Serializable]
    public class AndroidDeviceDescription
    {
        [SerializeField] private MobileTextureSubtarget m_TextureCompression = default(MobileTextureSubtarget);
        [SerializeField, BuildEnumFlags] private ScreenSizeRequirement m_Size = default(ScreenSizeRequirement);
        [SerializeField, BuildEnumFlags(2)] private ScreenDensityRequirement m_Density = default(ScreenDensityRequirement);


        public ScreenSizeRequirement Size
        {
            get { return m_Size; }
        }

        public ScreenDensityRequirement Density
        {
            get { return m_Density; }
        }

        public MobileTextureSubtarget TextureCompression
        {
            get { return m_TextureCompression; }
        }
    }
}
