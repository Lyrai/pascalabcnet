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
    }
}