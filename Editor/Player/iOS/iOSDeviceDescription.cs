using System;
using UnityEditor;
using UnityEngine;

namespace Sweet.BuildTools.Editor
{
    [Serializable]
    public class iOSDeviceDescription
    {
        [SerializeField, BuildEnumFlags] private IdiomDeviceRequirement m_Idiom = default(IdiomDeviceRequirement);
        [SerializeField, BuildEnumFlags] private ScaleDeviceRequirement m_Scale = default(ScaleDeviceRequirement);
        [SerializeField, BuildEnumFlags] private GraphicsDeviceRequirement m_Graphics = default(GraphicsDeviceRequirement);


        public IdiomDeviceRequirement Idiom
        {
            get { return m_Idiom; }
        }

        public ScaleDeviceRequirement Scale
        {
            get { return m_Scale; }
        }

        public GraphicsDeviceRequirement Graphics
        {
            get { return m_Graphics; }
        }


        public static explicit operator iOSDeviceRequirement(iOSDeviceDescription description)
        {
            return new iOSDeviceRequirement()
                .SetIdiomRequirement(description.Idiom)
                .SetScaleRequirement(description.Scale)
                .SetGraphicsRequirement(description.Graphics);
        }

        public static explicit operator iOSDeviceDescription(iOSDeviceRequirement requirement)
        {
            return new iOSDeviceDescription
            {
                m_Idiom = requirement.GetIdiomRequirement(),
                m_Scale = requirement.GetScaleRequirement(),
                m_Graphics = requirement.GetGraphicsRequirement()
            };
        }
    }
}
