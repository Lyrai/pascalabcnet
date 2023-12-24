using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkPropertyBuilderAdapter: FrameworkPropertyInfoAdapter, IPropertyBuilderAdapter
    {
        public new PropertyBuilder Adaptee { get; }
        
        public FrameworkPropertyBuilderAdapter(PropertyBuilder builder) : base(builder)
        {
            Adaptee = builder;
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            Adaptee.SetCustomAttribute((attribute as FrameworkCustomAttributeBuilderAdapter).Adaptee);
        }

        public void SetGetMethod(IMethodBuilderAdapter method)
        {
            Adaptee.SetGetMethod((method as FrameworkMethodBuilderAdapter).Adaptee);
        }

        public void SetSetMethod(IMethodBuilderAdapter method)
        {
            Adaptee.SetSetMethod((method as FrameworkMethodBuilderAdapter).Adaptee);
        }
    }
}