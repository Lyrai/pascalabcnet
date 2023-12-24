using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkModuleAdapter: IModuleAdapter
    {
        private Module Adaptee { get; }

        public FrameworkModuleAdapter(Module module)
        {
            Adaptee = module;
        }
        
        public ITypeAdapter GetType(string name)
        {
            return Adaptee.GetType(name).GetAdapter();
        }
    }
}