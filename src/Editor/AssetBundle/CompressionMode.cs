using System;
using System.Linq;
using System.Reflection;


namespace SweetEditor.Build
{
    public static class CompressionMode
    {
        private static ICompressionMode[] _Modes;
        private static string[] _ModeNames;




        public static string[] ModeNames
        {
            get { return _ModeNames; }
        }




        static CompressionMode()
        {
            Type interfaceType = typeof(ICompressionMode);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();


            _Modes = assemblies.SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && interfaceType.IsAssignableFrom(p))
                .Select(t => (ICompressionMode)Activator.CreateInstance(t))
                .ToArray();

            _ModeNames = _Modes.Select(m => m.Name).ToArray();
        }




        public static ICompressionMode GetCompressionMode(string mode)
        {
            return _Modes.FirstOrDefault(m => string.Equals(m.Name, mode, StringComparison.OrdinalIgnoreCase));
        }
    }
}