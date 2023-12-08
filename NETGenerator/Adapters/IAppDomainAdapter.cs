using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IAppDomainAdapter
    {
        IAssemblyBuilderAdapter DefineDynamicAssembly(AssemblyName name, string path);
    }
}