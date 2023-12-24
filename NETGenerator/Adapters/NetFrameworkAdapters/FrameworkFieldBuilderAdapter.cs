using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkFieldBuilderAdapter: FrameworkFieldInfoAdapter, IFieldBuilderAdapter
    {
        public bool IsStatic => Adaptee.IsStatic;
        
        public new FieldBuilder Adaptee { get; }
        
        public FrameworkFieldBuilderAdapter(FieldBuilder builder): base(builder)
        {
            Adaptee = builder;
        }

        public void SetConstant(object value)
        {
            Adaptee.SetConstant(value);
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            Adaptee.SetCustomAttribute((attribute as FrameworkCustomAttributeBuilderAdapter).Adaptee);
        }
    }
}