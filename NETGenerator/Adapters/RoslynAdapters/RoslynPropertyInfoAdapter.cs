using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynPropertyInfoAdapter: RoslynMemberInfoAdapter, IPropertyInfoAdapter
    {
        public string Name { get; }
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
    }
}