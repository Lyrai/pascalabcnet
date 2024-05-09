using System;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkEnumBuilderAdapter: FrameworkTypeAdapter, IEnumBuilderAdapter
    {
        public new EnumBuilder Adaptee { get; }
        
        public FrameworkEnumBuilderAdapter(EnumBuilder builder) : base(builder)
        {
            Adaptee = builder;
        }

        public ITypeAdapter CreateType()
        {
            return Adaptee.CreateType().GetAdapter();
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            Adaptee.SetCustomAttribute((attribute as FrameworkCustomAttributeBuilderAdapter).Adaptee);
        }

        public void DefineLiteral(string name, object value)
        {
            Adaptee.DefineLiteral(name, value);
        }
    }
}