using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace SweetEditor.Build
{
    [CreateAssetMenu(menuName = "Build Settings/Player/WebGL", order = -7)]
    public sealed class WebGLPlayerBuildSettings : PlayerBuildSettings
    {
        [Header("WebGL"), SerializeField] private int m_MemorySize = default(int);
        [SerializeField] private bool m_DataCaching = default(bool);
        [SerializeField, WebGLTemplateField] private string m_Template = "APPLICATION:Default";
        [SerializeField] private WebGLExceptionSupport m_ExceptionSupport = default(WebGLExceptionSupport);
#if UNITY_5_5_OR_NEWER
        [SerializeField] private WebGLCompressionFormat m_CompressionFormat = default(WebGLCompressionFormat);
        [SerializeField] private bool m_DebugSymbols = default(bool);
#endif




        public override BuildTarget BuildTarget
        {
            get { return BuildTarget.WebGL; }
        }



        protected override void Reset()
        {
            base.Reset();

#if UNITY_5_5_OR_NEWER
            m_MemorySize = PlayerSettings.WebGL.memorySize;
            m_DataCaching = PlayerSettings.WebGL.dataCaching;
            m_Template = PlayerSettings.WebGL.template;
            m_CompressionFormat = PlayerSettings.WebGL.compressionFormat;
            m_ExceptionSupport = PlayerSettings.WebGL.exceptionSupport;
            m_DebugSymbols = PlayerSettings.WebGL.debugSymbols;
#else
	        m_MemorySize = PlayerSettings.GetPropertyInt("memorySize", BuildTargetGroup.WebGL);
	        m_DataCaching = PlayerSettings.GetPropertyBool("dataCaching", BuildTargetGroup.WebGL);
	        m_Template = PlayerSettings.GetPropertyString("template", BuildTargetGroup.WebGL);
	        m_ExceptionSupport = (WebGLExceptionSupport)PlayerSettings.GetPropertyInt("exceptionSupport", BuildTargetGroup.WebGL);
#endif
        }


        protected override void OnPushPlayerSettings(Dictionary<string, object> settingsCache)
        {
#if UNITY_5_5_OR_NEWER
            settingsCache["memorySize"] = PlayerSettings.WebGL.memorySize;
            settingsCache["dataCaching"] = PlayerSettings.WebGL.dataCaching;
            settingsCache["template"] = PlayerSettings.WebGL.template;
            settingsCache["compressionFormat"] = PlayerSettings.WebGL.compressionFormat;
            settingsCache["exceptionSupport"] = PlayerSettings.WebGL.exceptionSupport;
            settingsCache["debugSymbols"] = PlayerSettings.WebGL.debugSymbols;

            PlayerSettings.WebGL.memorySize = m_MemorySize;
            PlayerSettings.WebGL.dataCaching = m_DataCaching;
            PlayerSettings.WebGL.template = m_Template;
            PlayerSettings.WebGL.compressionFormat = m_CompressionFormat;
            PlayerSettings.WebGL.exceptionSupport = m_ExceptionSupport;
            PlayerSettings.WebGL.debugSymbols = m_DebugSymbols;
#else
            settingsCache["memorySize"] = PlayerSettings.GetPropertyInt("memorySize", BuildTargetGroup.WebGL);
            settingsCache["dataCaching"] = PlayerSettings.GetPropertyBool("dataCaching", BuildTargetGroup.WebGL);
            settingsCache["template"] = PlayerSettings.GetPropertyString("template", BuildTargetGroup.WebGL);
            settingsCache["exceptionSupport"] = PlayerSettings.GetPropertyInt("exceptionSupport", BuildTargetGroup.WebGL);

            PlayerSettings.SetPropertyInt("memorySize", m_MemorySize, BuildTargetGroup.WebGL);
            PlayerSettings.SetPropertyBool("dataCaching", m_DataCaching, BuildTargetGroup.WebGL);
            PlayerSettings.SetPropertyString("template",
                string.IsNullOrEmpty(m_Template) ? "APPLICATION:Default" : m_Template, BuildTargetGroup.WebGL);
            PlayerSettings.SetPropertyInt("exceptionSupport", (int)m_ExceptionSupport, BuildTargetGroup.WebGL);
#endif
        }


        protected override void OnPopPlayerSettings(Dictionary<string, object> settingsCache)
        {
#if UNITY_5_5_OR_NEWER
            TrySetValue<int>((v) => PlayerSettings.WebGL.memorySize = v, "memorySize", settingsCache);
            TrySetValue<bool>((v) => PlayerSettings.WebGL.dataCaching = v, "dataCaching", settingsCache);
            TrySetValue<string>((v) => PlayerSettings.WebGL.template = v, "template", settingsCache);
            TrySetValue<WebGLCompressionFormat>((v) => PlayerSettings.WebGL.compressionFormat = v, "compressionFormat", settingsCache);
            TrySetValue<WebGLExceptionSupport>((v) => PlayerSettings.WebGL.exceptionSupport = v, "exceptionSupport", settingsCache);
            TrySetValue<bool>((v) => PlayerSettings.WebGL.debugSymbols = v, "debugSymbols", settingsCache);
#else
	        TrySetValue<int>((v) =>  PlayerSettings.SetPropertyInt("memorySize", v, BuildTargetGroup.WebGL), "memorySize", settingsCache);
	        TrySetValue<bool>((v) =>  PlayerSettings.SetPropertyBool("dataCaching", v, BuildTargetGroup.WebGL), "dataCaching", settingsCache);
	        TrySetValue<string>((v) =>  PlayerSettings.SetPropertyString("template", v, BuildTargetGroup.WebGL), "template", settingsCache);
	        TrySetValue<int>((v) => PlayerSettings.SetPropertyInt("exceptionSupport", v, BuildTargetGroup.WebGL), "exceptionSupport", settingsCache);
#endif
        }
    }


#if !UNITY_5_5_OR_NEWER
    public enum WebGLExceptionSupport
    {
        None,
        ExplictelyThrownExceptionsOnly,
        Full
    }
#endif
}