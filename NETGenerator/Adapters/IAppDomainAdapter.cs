using System.Reflection;

namespace NETGenerator.Adapters
{
    public interface IAppDomainAdapter
    {
        IAssemblyBuilderAdapter DefineDynamicAssembly(AssemblyName name, string path);
    }
}