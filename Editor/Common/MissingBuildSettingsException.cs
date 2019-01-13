using System;


namespace Sweet.BuildTools.Editor
{
    public class MissingBuildSettingsException : Exception
    {
        public MissingBuildSettingsException(Type type, string id)
            : base(string.Format("Could not locate a {0} object with id {1}", type.Name, id))
        {

        }
    }
}