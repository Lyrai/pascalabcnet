using System;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynLocalBuilderAdapter: ILocalBuilderAdapter
    {
        public ITypeAdapter LocalType { get; }
        public bool Pinned { get; }
        public string Name { get; private set; }

        public RoslynLocalBuilderAdapter(ITypeAdapter localType, bool pinned)
        {
            LocalType = localType;
            Pinned = pinned;
        }

        public void SetLocalSymInfo(string name)
        {
            Name = name;
        }
    }
}