#if UNITY_5_5_OR_NEWER || UNITY_5_4_4
#define AUTOMATIC_SIGNING
#endif

using System;


namespace SweetEditor.Build
{
    [Serializable]
    public struct InfoPlistEntry
    {
        public string Key;
        public string Value;
    }
}