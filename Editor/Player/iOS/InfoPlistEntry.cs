using System;


namespace Sweet.BuildTools.Editor
{
    [Serializable]
    public struct InfoPlistEntry
    {
        public PlistEntryType Type;
        public string Key;
        public string StringValue;
        public bool BooleanValue;
        public int IntegerValue;
    }
}