using System;
using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkAppDomainAdapter: IAppDomainAdapter
    {
        private AppDomain _adaptee;

        public FrameworkAppDomainAdapter(AppDomain adaptee)
        {
            _adaptee = adaptee;
        }
        
        public IAssemblyBuilderAdapter DefineDynamicAssembly(AssemblyName name, string path)
        {
            return _adaptee.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save, path).GetAdapter();
        }
    }
}