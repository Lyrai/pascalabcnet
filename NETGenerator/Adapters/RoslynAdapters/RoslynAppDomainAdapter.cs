using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynAppDomainAdapter: IAppDomainAdapter
    {
        public IAssemblyBuilderAdapter DefineDynamicAssembly(AssemblyName name, string path)
        {
            return AdapterFactory.AssemblyBuilder(name.Name, path);
        }
    }
}