using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynFieldBuilderAdapter: RoslynFieldInfoAdapter, IFieldBuilderAdapter
    {
        public RoslynFieldBuilderAdapter(ITypeAdapter declaringType, string name, ITypeAdapter type,
            FieldAttributes attributes) : base(declaringType, name, type, attributes)
        {
        }

        public void SetConstant(object value)
        {
            Console.WriteLine("RoslynFieldBuilderAdapter.SetConstant not implemented");
            //throw new System.NotImplementedException();
        }

        public override IMemberInfoAdapter Instantiate(Dictionary<ITypeAdapter, ITypeAdapter> typeArguments,
            ITypeAdapter declaringType)
        {
            return typeArguments.TryGetValue(FieldType, out var instance)
                ? new RoslynFieldBuilderAdapter(declaringType, Name, instance, Attributes)
                : new RoslynFieldBuilderAdapter(declaringType, Name, FieldType, Attributes);
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            Console.WriteLine("RoslynFieldBuilderAdapter.SetCustomAttribute not implemented");
            //throw new System.NotImplementedException();
        }
    }
}