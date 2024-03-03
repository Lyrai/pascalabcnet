using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynFieldBuilderAdapter: RoslynFieldInfoAdapter, IFieldBuilderAdapter
    {
        public RoslynFieldBuilderAdapter(string name, ITypeAdapter type, FieldAttributes attributes) : base(name, type, attributes)
        { }

        public void SetConstant(object value)
        {
            throw new System.NotImplementedException();
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            throw new System.NotImplementedException();
        }
    }
}