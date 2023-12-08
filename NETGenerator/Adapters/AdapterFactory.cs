using System;
using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public abstract class AdapterFactory
    {
        private static AdapterFactory _instance = null;
        
        public static IAppDomainAdapter AppDomain()
        {
            return Instance().CreateAppDomain();
        }

        public static TypeAdapter Type(Type type)
        {
            return Instance().CreateType(type);
        }

        public static IMethodInfoAdapter MethodInfo(MethodInfo info)
        {
            return Instance().CreateMethodInfo(info);
        }
        
        public static IFieldInfoAdapter FieldInfo(FieldInfo info)
        {
            return Instance().CreateFieldInfo(info);
        }

        public static ILocalBuilderAdapter LocalBuilder(LocalBuilder builder)
        {
            return Instance().CreateLocalBuilder(builder);
        }
        
        public static IFieldBuilderAdapter FieldBuilder(FieldBuilder builder)
        {
            return Instance().CreateFieldBuilder(builder);
        }

        public static IConstructorInfoAdapter ConstructorInfo(ConstructorInfo info)
        {
            return Instance().CreateConstructorInfo(info);
        }

        public static IParameterBuilderAdapter ParameterBuilder(ParameterBuilder builder)
        {
            return Instance().CreateParameterBuilder(builder);
        }

        public static ICustomAttributeBuilderAdapter CustomAttributeBuilder(IConstructorInfoAdapter constructor, object[] args)
        {
            return Instance().CreateCustomAttributeBuilder(constructor, args);
        }
        
        public static ICustomAttributeBuilderAdapter CustomAttributeBuilder(IConstructorInfoAdapter constructor, object[] constructorArgs, IPropertyInfoAdapter[] namedProperties, object[] propertyValues)
        {
            return Instance().CreateCustomAttributeBuilder(constructor, constructorArgs, namedProperties, propertyValues);
        }

        public static ICustomAttributeBuilderAdapter CustomAttributeBuilder(IConstructorInfoAdapter constructor,
            object[] constructorArgs, IPropertyInfoAdapter[] namedProperties, object[] propertyValues,
            IFieldInfoAdapter[] fields, object[] fieldValues)
        {
            return Instance().CreateCustomAttributeBuilder(constructor, constructorArgs, namedProperties,
                propertyValues, fields, fieldValues);
        }

        public static IPropertyInfoAdapter PropertyInfo(PropertyInfo info)
        {
            return Instance().CreatePropertyInfo(info);
        }
        
        public static IPropertyBuilderAdapter PropertyBuilder(PropertyBuilder builder)
        {
            return Instance().CreatePropertyBuilder(builder);
        }
        
        public static IEventBuilderAdapter EventBuilder(EventBuilder builder)
        {
            return Instance().CreateEventBuilder(builder);
        }

        public static IAssemblyBuilderAdapter AssemblyBuilder(AssemblyBuilder builder)
        {
            return Instance().CreateAssemblyBuilder(builder);
        }

        protected abstract IAppDomainAdapter CreateAppDomain();
        protected abstract TypeAdapter CreateType(Type type);
        protected abstract IMethodInfoAdapter CreateMethodInfo(MethodInfo info);
        protected abstract IFieldInfoAdapter CreateFieldInfo(FieldInfo info);
        protected abstract ILocalBuilderAdapter CreateLocalBuilder(LocalBuilder builder);
        protected abstract IFieldBuilderAdapter CreateFieldBuilder(FieldBuilder builder);
        protected abstract IParameterBuilderAdapter CreateParameterBuilder(ParameterBuilder builder);
        protected abstract IEventBuilderAdapter CreateEventBuilder(EventBuilder builder);
        protected abstract IAssemblyBuilderAdapter CreateAssemblyBuilder(AssemblyBuilder builder);
        protected abstract ICustomAttributeBuilderAdapter CreateCustomAttributeBuilder(IConstructorInfoAdapter constructor, object[] args);
        protected abstract ICustomAttributeBuilderAdapter CreateCustomAttributeBuilder(IConstructorInfoAdapter constructor, object[] constructorArgs, IPropertyInfoAdapter[] namedProperties, object[] propertyValues);

        protected abstract ICustomAttributeBuilderAdapter CreateCustomAttributeBuilder(
            IConstructorInfoAdapter constructor,
            object[] constructorArgs, IPropertyInfoAdapter[] namedProperties, object[] propertyValues,
            IFieldInfoAdapter[] fields, object[] fieldValues);
        protected abstract IPropertyInfoAdapter CreatePropertyInfo(PropertyInfo info);
        protected abstract IPropertyBuilderAdapter CreatePropertyBuilder(PropertyBuilder builder);
        protected abstract IConstructorInfoAdapter CreateConstructorInfo(ConstructorInfo info);

        private static AdapterFactory Instance()
        {
            return _instance ?? null;
        }
    }

    public static class AdapterExtensions
    {
        public static TypeAdapter GetAdapter(this Type type)
        {
            return AdapterFactory.Type(type);
        }

        public static IConstructorInfoAdapter GetAdapter(this ConstructorInfo constructor)
        {
            return AdapterFactory.ConstructorInfo(constructor);
        }

        public static IMethodInfoAdapter GetAdapter(this MethodInfo info)
        {
            return AdapterFactory.MethodInfo(info);
        }

        public static IFieldInfoAdapter GetAdapter(this FieldInfo info)
        {
            return AdapterFactory.FieldInfo(info);
        }

        public static IPropertyInfoAdapter GetAdapter(this PropertyInfo info)
        {
            return AdapterFactory.PropertyInfo(info);
        }

        public static IParameterBuilderAdapter GetAdapter(this ParameterBuilder builder)
        {
            return AdapterFactory.ParameterBuilder(builder);
        }

        public static IEventBuilderAdapter GetAdapter(this EventBuilder builder)
        {
            return AdapterFactory.EventBuilder(builder);
        }
        
        public static IAssemblyBuilderAdapter GetAdapter(this AssemblyBuilder builder)
        {
            return AdapterFactory.AssemblyBuilder(builder);
        }
    }
}