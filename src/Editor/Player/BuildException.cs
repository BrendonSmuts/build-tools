using System;

namespace SweetEditor.Build
{
    public class BuildException : Exception
    {
        public BuildException(string msg)
            : base(msg)
        {
            
        }
    }
}