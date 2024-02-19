using System;
using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynAdapterFactory: AdapterFactory
    {
        protected override IAppDomainAdapter CreateAppDomain()
        {
            return new RoslynAppDomainAdapter();
        }

        protected override ITypeAdapter CreateType(Type type)
        {
            throw new NotImplementedException();
        }

        protected override ITypeBuilderAdapter CreateTypeBuilder(TypeBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override IEnumBuilderAdapter CreateEnumBuilder(EnumBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override IMethodInfoAdapter CreateMethodInfo(MethodInfo info)
        {
            throw new NotImplementedException();
        }

        protected override IMethodBuilderAdapter CreateMethodBuilder(MethodBuilder builder)
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

        protected override IParameterInfoAdapter CreateParameterInfo(ParameterInfo info)
        {
            throw new NotImplementedException();
        }

        protected override IEventBuilderAdapter CreateEventBuilder(EventBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override IModuleBuilderAdapter CreateModuleBuilder(ModuleBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override IAssemblyBuilderAdapter CreateAssemblyBuilder(AssemblyBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override IGenericTypeParameterBuilderAdapter CreateGenericTypeParameterBuilder(GenericTypeParameterBuilder builder)
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

        protected override IConstructorBuilderAdapter CreateConstructorBuilder(ConstructorBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override IMemberInfoAdapter CreateMemberInfo(MemberInfo info)
        {
            throw new NotImplementedException();
        }

        protected override IILGeneratorAdapter CreateILGenerator(ILGenerator generator)
        {
            throw new NotImplementedException();
        }

        protected override AssemblyAdapter CreateAssembly(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        protected override IModuleAdapter CreateModule(Module module)
        {
            throw new NotImplementedException();
        }
    }
}