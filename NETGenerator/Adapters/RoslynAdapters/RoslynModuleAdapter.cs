using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynModuleAdapter: IModuleAdapter
    {
        public AssemblyAdapter Assembly { get; }
        
        protected Dictionary<string, ITypeAdapter> _members;

        public RoslynModuleAdapter(AssemblyAdapter assembly)
        {
            Assembly = assembly;
            _members = new Dictionary<string, ITypeAdapter>();
        }
        
        public ITypeAdapter GetType(string name)
        {
            return _members.TryGetValue(name, out var type) ? type : null;
        }

        public ITypeAdapter[] GetTypes()
        {
            return _members.Values.ToArray();
        }
    }
}