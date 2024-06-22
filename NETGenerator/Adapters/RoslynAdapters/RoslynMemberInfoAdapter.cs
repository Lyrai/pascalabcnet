﻿using System.Collections.Generic;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public abstract class RoslynMemberInfoAdapter: IMemberInfoAdapter
    {
        public abstract string Name { get; }
        public abstract bool IsPublic { get; }
        public abstract bool IsPrivate { get; }
        public abstract bool IsAssembly { get; }
        public abstract bool IsFamily { get; }
        public abstract bool IsFamilyAndAssembly { get; }
        public abstract bool IsFamilyOrAssembly { get; }

        public abstract IMemberInfoAdapter Instantiate(Dictionary<ITypeAdapter, ITypeAdapter> typeArguments,
            ITypeAdapter declaringType);
    }
}