using System;
using System.Collections.Generic;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynPropertyInfoAdapter: RoslynMemberInfoAdapter, IPropertyInfoAdapter
    {
        public override string Name { get; }
        public override bool IsPublic => true;
        public override bool IsPrivate => false;
        public override bool IsAssembly => false;
        public override bool IsFamily => false;
        public override bool IsFamilyAndAssembly => false;
        public override bool IsFamilyOrAssembly => false;
        public PropertyAttributes Attributes { get; }
        public ITypeAdapter PropertyType { get; protected set; }
        
        protected IMethodInfoAdapter _getMethod = null;
        protected IMethodInfoAdapter _setMethod = null;

        public RoslynPropertyInfoAdapter(string name, PropertyAttributes attributes)
        {
            Name = name;
            Attributes = attributes;
        }
        
        public IMethodInfoAdapter GetGetMethod()
        {
            return _getMethod;
        }

        public IMethodInfoAdapter GetSetMethod()
        {
            return _setMethod;
        }

        public override IMemberInfoAdapter Instantiate(Dictionary<ITypeAdapter, ITypeAdapter> typeArguments,
            ITypeAdapter declaringType)
        {
            throw new NotSupportedException();
        }
    }
}