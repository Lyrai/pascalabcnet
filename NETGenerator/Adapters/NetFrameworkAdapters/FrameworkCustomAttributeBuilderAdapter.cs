using System.Linq;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkCustomAttributeBuilderAdapter: ICustomAttributeBuilderAdapter
    {
        public CustomAttributeBuilder Adaptee { get; }

        public FrameworkCustomAttributeBuilderAdapter(IConstructorInfoAdapter constructor, object[] args)
        {
            Adaptee = new CustomAttributeBuilder((constructor as FrameworkConstructorInfoAdapter).Adaptee, args);
        }
        
        public FrameworkCustomAttributeBuilderAdapter(IConstructorInfoAdapter constructor,
            object[] constructorArgs, IPropertyInfoAdapter[] namedProperties, object[] propertyValues)
        {
            Adaptee = new CustomAttributeBuilder(
                (constructor as FrameworkConstructorInfoAdapter).Adaptee,
                constructorArgs,
                namedProperties.Select(p => (p as FrameworkPropertyInfoAdapter).Adaptee).ToArray(),
                propertyValues);
        }
        
        public FrameworkCustomAttributeBuilderAdapter(IConstructorInfoAdapter constructor,
            object[] constructorArgs, IPropertyInfoAdapter[] namedProperties, object[] propertyValues,
            IFieldInfoAdapter[] fields, object[] fieldValues)
        {
            Adaptee = new CustomAttributeBuilder(
                (constructor as FrameworkConstructorInfoAdapter).Adaptee,
                constructorArgs,
                namedProperties.Select(p => (p as FrameworkPropertyInfoAdapter).Adaptee).ToArray(),
                propertyValues,
                fields.Select(f => (f as FrameworkFieldInfoAdapter).Adaptee).ToArray(),
                fieldValues);
        }
    }
}