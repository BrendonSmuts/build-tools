using System;

namespace Sweet.BuildTools.Editor
{
    public class BuildException : Exception
    {
        public BuildException(string msg)
            : base(msg)
        {
            
        }
    }
}