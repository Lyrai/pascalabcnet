using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using PascalABCCompiler.NETGenerator.Adapters;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal static class SymbolTable
    {
        private static Dictionary<IAdapter, Symbol> _dict = new Dictionary<IAdapter, Symbol>();

        public static void Init()
        {
            _dict.Clear();
        }

        public static void Set(IAdapter adapter, Symbol symbol)
        {
            _dict[adapter] = symbol;
        }

        public static Symbol Get(IAdapter adapter)
        {
            return _dict[adapter];
        }
    }
}