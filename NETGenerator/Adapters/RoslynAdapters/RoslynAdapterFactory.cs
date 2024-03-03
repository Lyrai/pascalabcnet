using System;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CodeAnalysis.CSharp.Emit;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynAdapterFactory: AdapterFactory
    {
        protected override IAppDomainAdapter CreateAppDomain()
        {
            return new RoslynAppDomainAdapter();
        }

        protected override ITypeAdapter CreateType(Type type)
        {
            return new FrameworkTypeAdapter(type);
        }

        protected override ITypeBuilderAdapter CreateTypeBuilder(TypeBuilder builder)
        {
            throw new NotSupportedException();
        }

        protected override IEnumBuilderAdapter CreateEnumBuilder(EnumBuilder builder)
        {
            throw new NotSupportedException();
        }

        protected override IMethodInfoAdapter CreateMethodInfo(MethodInfo info)
        {
            return new FrameworkMethodInfoAdapter(info);
        }

        protected override IMethodBuilderAdapter CreateMethodBuilder(MethodBuilder builder)
        {
            throw new NotSupportedException();
        }

        protected override IFieldInfoAdapter CreateFieldInfo(FieldInfo info)
        {
            return new FrameworkFieldInfoAdapter(info);
        }

        protected override ILocalBuilderAdapter CreateLocalBuilder(LocalBuilder builder)
        {
            throw new NotSupportedException();
        }

        protected override IFieldBuilderAdapter CreateFieldBuilder(FieldBuilder builder)
        {
            throw new NotSupportedException();
        }

        protected override IParameterBuilderAdapter CreateParameterBuilder(ParameterBuilder builder)
        {
            throw new NotSupportedException();
        }

        protected override IParameterInfoAdapter CreateParameterInfo(ParameterInfo info)
        {
            return new FrameworkParameterInfoAdapter(info);
        }

        protected override IEventBuilderAdapter CreateEventBuilder(EventBuilder builder)
        {
            throw new NotSupportedException();
        }

        protected override IModuleBuilderAdapter CreateModuleBuilder(ModuleBuilder builder)
        {
            throw new NotSupportedException();
        }

        protected override IAssemblyBuilderAdapter CreateAssemblyBuilder(AssemblyBuilder builder)
        {
            throw new NotSupportedException();
        }

        protected override IGenericTypeParameterBuilderAdapter CreateGenericTypeParameterBuilder(GenericTypeParameterBuilder builder)
        {
            throw new NotSupportedException();
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
            return new FrameworkPropertyInfoAdapter(info);
        }

        protected override IPropertyBuilderAdapter CreatePropertyBuilder(PropertyBuilder builder)
        {
            throw new NotSupportedException();
        }

        protected override IConstructorInfoAdapter CreateConstructorInfo(ConstructorInfo info)
        {
            return new FrameworkConstructorInfoAdapter(info);
        }

        protected override IConstructorBuilderAdapter CreateConstructorBuilder(ConstructorBuilder builder)
        {
            throw new NotSupportedException();
        }

        protected override IMemberInfoAdapter CreateMemberInfo(MemberInfo info)
        {
            return new FrameworkMemberInfoAdapter(info);
        }

        protected override IILGeneratorAdapter CreateILGenerator(ILGenerator generator)
        {
            throw new NotSupportedException();
        }

        protected override AssemblyAdapter CreateAssembly(Assembly assembly)
        {
            return new FrameworkAssemblyAdapter(assembly);
        }

        protected override IModuleAdapter CreateModule(Module module)
        {
            return new FrameworkModuleAdapter(module);
        }
    }
}