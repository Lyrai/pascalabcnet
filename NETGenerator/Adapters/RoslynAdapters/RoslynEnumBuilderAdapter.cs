using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynEnumBuilderAdapter: RoslynTypeAdapter, IEnumBuilderAdapter
    {
        public RoslynEnumBuilderAdapter(IModuleAdapter module, string name, TypeAttributes attr, ITypeAdapter parent) : base(module, name, attr, parent, null)
        {
        }

        public ITypeAdapter CreateType()
        {
            throw new System.NotImplementedException();
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            throw new System.NotImplementedException();
        }

        public void DefineLiteral(string name, object value)
        {
            throw new System.NotImplementedException();
        }
    }
}