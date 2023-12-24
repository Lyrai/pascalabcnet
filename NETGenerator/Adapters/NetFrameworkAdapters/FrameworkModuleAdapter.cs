using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkModuleAdapter: IModuleAdapter
    {
        public Module Adaptee { get; }

        public FrameworkModuleAdapter(Module module)
        {
            Adaptee = module;
        }
        
        public ITypeAdapter GetType(string name)
        {
            return Adaptee.GetType(name).GetAdapter();
        }
        
        public override int GetHashCode()
        {
            return Adaptee.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FrameworkModuleAdapter module))
            {
                return false;
            }

            return Adaptee.Equals(module.Adaptee);
        }
    }
}