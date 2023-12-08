using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkAdapterFactory: AdapterFactory
    {
        protected override IAppDomainAdapter CreateAppDomain()
        {
            return new FrameworkAppDomainAdapter(Thread.GetDomain());
        }

        protected override TypeAdapter CreateType(Type type)
        {
            throw new NotImplementedException();
        }

        protected override IMethodInfoAdapter CreateMethodInfo(MethodInfo info)
        {
            throw new NotImplementedException();
        }

        protected override IFieldInfoAdapter CreateFieldInfo(FieldInfo info)
        {
            throw new NotImplementedException();
        }

        protected override ILocalBuilderAdapter CreateLocalBuilder(LocalBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override IFieldBuilderAdapter CreateFieldBuilder(FieldBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override IParameterBuilderAdapter CreateParameterBuilder(ParameterBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override IEventBuilderAdapter CreateEventBuilder(EventBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override IAssemblyBuilderAdapter CreateAssemblyBuilder(AssemblyBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override ICustomAttributeBuilderAdapter CreateCustomAttributeBuilder(IConstructorInfoAdapter constructor, object[] args)
        {
            throw new NotImplementedException();
        }

        protected override ICustomAttributeBuilderAdapter CreateCustomAttributeBuilder(IConstructorInfoAdapter constructor,
            object[] constructorArgs, IPropertyInfoAdapter[] namedProperties, object[] propertyValues)
        {
            throw new NotImplementedException();
        }

        protected override ICustomAttributeBuilderAdapter CreateCustomAttributeBuilder(IConstructorInfoAdapter constructor,
            object[] constructorArgs, IPropertyInfoAdapter[] namedProperties, object[] propertyValues,
            IFieldInfoAdapter[] fields, object[] fieldValues)
        {
            throw new NotImplementedException();
        }

        protected override IPropertyInfoAdapter CreatePropertyInfo(PropertyInfo info)
        {
            throw new NotImplementedException();
        }

        protected override IPropertyBuilderAdapter CreatePropertyBuilder(PropertyBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override IConstructorInfoAdapter CreateConstructorInfo(ConstructorInfo info)
        {
            throw new NotImplementedException();
        }
    }
}