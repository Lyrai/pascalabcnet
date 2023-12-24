using System.Linq;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkAssemblyAdapter: AssemblyAdapter
    {
        public Assembly Adaptee { get; }

        public FrameworkAssemblyAdapter(Assembly assembly)
        {
            Adaptee = assembly;
        }
        
        public override ITypeAdapter[] GetTypes()
        {
            return Adaptee.GetTypes().Select(t => t.GetAdapter()).ToArray();
        }

        public override ITypeAdapter GetType(string name, bool throwOnError)
        {
            return Adaptee.GetType(name, throwOnError).GetAdapter();
        }
        
        public override int GetHashCode()
        {
            return Adaptee.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FrameworkAssemblyAdapter adapter))
            {
                return false;
            }

            return Adaptee.Equals(adapter.Adaptee);
        }
    }
}