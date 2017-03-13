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
            List<Type> assemblyTypes = new List<Type>();

            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];

                try
                {
                    Type[] types = assembly.GetTypes();
                    assemblyTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException e)
                {
                    assemblyTypes.AddRange(e.Types.Where(t => t != null));
                }
            }

            _Modes = assemblyTypes
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
