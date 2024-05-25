using System.Collections.Generic;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IMemberInfoAdapter: IAdapter
    {
        string Name { get; }
        bool IsPublic { get; }
        bool IsPrivate { get; }
        bool IsAssembly { get; }
        bool IsFamily { get; }
        bool IsFamilyAndAssembly { get; }
        bool IsFamilyOrAssembly { get; }

        IMemberInfoAdapter Instantiate(Dictionary<ITypeAdapter, ITypeAdapter> typeArguments, ITypeAdapter declaringType);
    }
}