using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IAppDomainAdapter: IAdapter
    {
        IAssemblyBuilderAdapter DefineDynamicAssembly(AssemblyName name, string path);
    }
}