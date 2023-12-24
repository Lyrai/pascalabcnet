using System;
using System.Collections.Generic;
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
        
        protected override IGenericTypeParameterBuilderAdapter CreateGenericTypeParameterBuilder(GenericTypeParameterBuilder builder)
        {
            return new FrameworkGenericTypeParameterBuilderAdapter(builder);
        }


        protected override ITypeAdapter CreateType(Type type)
        {
            return new FrameworkTypeAdapter(type);
        }

        protected override ITypeBuilderAdapter CreateTypeBuilder(TypeBuilder builder)
        {
            return new FrameworkTypeBuilderAdapter(builder);
        }

        protected override IEnumBuilderAdapter CreateEnumBuilder(EnumBuilder builder)
        {
            return new FrameworkEnumBuilderAdapter(builder);
        }

        protected override IMethodInfoAdapter CreateMethodInfo(MethodInfo info)
        {
            return new FrameworkMethodInfoAdapter(info);
        }

        protected override IMethodBuilderAdapter CreateMethodBuilder(MethodBuilder builder)
        {
            return new FrameworkMethodBuilderAdapter(builder);
        }

        protected override IFieldInfoAdapter CreateFieldInfo(FieldInfo info)
        {
            return new FrameworkFieldInfoAdapter(info);
        }

        protected override ILocalBuilderAdapter CreateLocalBuilder(LocalBuilder builder)
        {
            return new FrameworkLocalBuilderAdapter(builder);
        }

        protected override IFieldBuilderAdapter CreateFieldBuilder(FieldBuilder builder)
        {
            return new FrameworkFieldBuilderAdapter(builder);
        }

        protected override IParameterBuilderAdapter CreateParameterBuilder(ParameterBuilder builder)
        {
            return new FrameworkParameterBuilderAdapter(builder);
        }

        protected override IParameterInfoAdapter CreateParameterInfo(ParameterInfo info)
        {
            return new FrameworkParameterInfoAdapter(info);
        }

        protected override IEventBuilderAdapter CreateEventBuilder(EventBuilder builder)
        {
            return new FrameworkEventBuilderAdapter(builder);
        }

        protected override IAssemblyBuilderAdapter CreateAssemblyBuilder(AssemblyBuilder builder)
        {
            return new FrameworkAssemblyBuilderAdapter(builder);
        }
        
        protected override AssemblyAdapter CreateAssembly(Assembly assembly)
        {
            return new FrameworkAssemblyAdapter(assembly);
        }

        protected override IModuleAdapter CreateModule(Module module)
        {
            return new FrameworkModuleAdapter(module);
        }

        protected override IModuleBuilderAdapter CreateModuleBuilder(ModuleBuilder builder)
        {
            return new FrameworkModuleBuilderAdapter(builder);
        }

        protected override ICustomAttributeBuilderAdapter CreateCustomAttributeBuilder(IConstructorInfoAdapter constructor, object[] args)
        {
            return new FrameworkCustomAttributeBuilderAdapter(constructor, args);
        }

        protected override ICustomAttributeBuilderAdapter CreateCustomAttributeBuilder(IConstructorInfoAdapter constructor,
            object[] constructorArgs, IPropertyInfoAdapter[] namedProperties, object[] propertyValues)
        {
            return new FrameworkCustomAttributeBuilderAdapter(constructor, constructorArgs, namedProperties, propertyValues);
        }

        protected override ICustomAttributeBuilderAdapter CreateCustomAttributeBuilder(IConstructorInfoAdapter constructor,
            object[] constructorArgs, IPropertyInfoAdapter[] namedProperties, object[] propertyValues,
            IFieldInfoAdapter[] fields, object[] fieldValues)
        {
            return new FrameworkCustomAttributeBuilderAdapter(constructor, constructorArgs, namedProperties, propertyValues, fields, fieldValues);
        }

        protected override IPropertyInfoAdapter CreatePropertyInfo(PropertyInfo info)
        {
            return new FrameworkPropertyInfoAdapter(info);
        }

        protected override IPropertyBuilderAdapter CreatePropertyBuilder(PropertyBuilder builder)
        {
            return new FrameworkPropertyBuilderAdapter(builder);
        }

        protected override IConstructorInfoAdapter CreateConstructorInfo(ConstructorInfo info)
        {
            return new FrameworkConstructorInfoAdapter(info);
        }

        protected override IConstructorBuilderAdapter CreateConstructorBuilder(ConstructorBuilder builder)
        {
            return new FrameworkConstructorBuilderAdapter(builder);
        }

        protected override IMemberInfoAdapter CreateMemberInfo(MemberInfo info)
        {
            switch (info)
            {
                case PropertyBuilder propertyBuilder:
                    return propertyBuilder.GetAdapter();
                case PropertyInfo propertyInfo:
                    return propertyInfo.GetAdapter();
                case MethodInfo methodInfo:
                    return methodInfo.GetAdapter();
                case FieldBuilder fieldBuilder:
                    return fieldBuilder.GetAdapter();
                case FieldInfo fieldInfo:
                    return fieldInfo.GetAdapter();
                case ConstructorBuilder constructorBuilder:
                    return constructorBuilder.GetAdapter();
                case ConstructorInfo constructorInfo:
                    return constructorInfo.GetAdapter();
                default:
                    throw new ArgumentException("Invalid member type");
            }
        }

        protected override IILGeneratorAdapter CreateILGenerator(ILGenerator generator)
        {
            return new FrameworkILGeneratorAdapter(generator);
        }
    }
}