using System;
using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
#if !NETCOREAPP
    public class FrameworkAppDomainAdapter: IAppDomainAdapter
    {
        public AppDomain Adaptee { get; }

        public FrameworkAppDomainAdapter(AppDomain adaptee)
        {
            Adaptee = adaptee;
        }
        
        public IAssemblyBuilderAdapter DefineDynamicAssembly(AssemblyName name, string path)
        {
            return Adaptee.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save, path).GetAdapter();
        }
        
        public override int GetHashCode()
        {
            return Adaptee.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FrameworkAppDomainAdapter adapter))
            {
                return false;
            }

            return Adaptee.Equals(adapter.Adaptee);
        }
    }
#endif
}