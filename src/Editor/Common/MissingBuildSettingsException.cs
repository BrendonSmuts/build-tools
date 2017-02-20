using System;


namespace SweetEditor.Build
{
    public class MissingBuildSettingsException : Exception
    {
        public MissingBuildSettingsException(string id)
            : base(string.Format("Could not locate a PlayerBuildSettings object with id {0}", id))
        {

        }
    }
}