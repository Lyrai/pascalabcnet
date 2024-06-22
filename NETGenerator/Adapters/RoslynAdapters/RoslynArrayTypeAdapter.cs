using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynArrayTypeAdapter: RoslynTypeAdapter
    {
        public override bool IsArray => true;

        private ITypeAdapter _elementType;

        public RoslynArrayTypeAdapter(ITypeAdapter elementType)
            : base(elementType.Module, elementType.FullName + "[]", elementType.Attributes, null, null)
        {
            _elementType = elementType;
        }
        
        protected RoslynArrayTypeAdapter(IModuleAdapter module, string name, TypeAttributes attr, ITypeAdapter parent, ITypeAdapter[] interfaces) : base(module, name, attr, parent, interfaces)
        {
        }

        protected RoslynArrayTypeAdapter(ITypeBuilderAdapter declaringType, string name, TypeAttributes attr) : base(declaringType, name, attr)
        {
        }

        public override ITypeAdapter GetElementType()
        {
            return _elementType;
        }

        public override int GetArrayRank()
        {
            ITypeAdapter type = this;
            int count = 0;
            while (type is RoslynArrayTypeAdapter)
            {
                ++count;
                type = type.GetElementType();
            }

            return count;
        }
    }
}