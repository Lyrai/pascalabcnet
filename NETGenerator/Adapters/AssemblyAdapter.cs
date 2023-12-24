using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public abstract class AssemblyAdapter
    {
        public abstract ITypeAdapter[] GetTypes();
        public abstract ITypeAdapter GetType(string name, bool throwOnError);

        public static AssemblyAdapter Load(byte[] bytes)
        {
            return Assembly.Load(bytes).GetAdapter();
        }

        public static AssemblyAdapter LoadFrom(string filename)
        {
            return Assembly.LoadFrom(filename).GetAdapter();
        }
    }
}