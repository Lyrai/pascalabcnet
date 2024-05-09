using System;
using System.Collections.Generic;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynPropertyInfoAdapter: RoslynMemberInfoAdapter, IPropertyInfoAdapter
    {
        public override string Name { get; }
        public override bool IsPublic => true;
        public override bool IsPrivate => true;
        public PropertyAttributes Attributes { get; }
        
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
        
        public override IMemberInfoAdapter Instantiate(Dictionary<ITypeAdapter, ITypeAdapter> typeArguments,
            ITypeAdapter declaringType)
        {
            throw new NotSupportedException();
        }
    }
}