using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CodeAnalysis.CSharp.Emit;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;
using PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    internal abstract class AdapterFactory
    {
        private static AdapterFactory _instance = null;

        public static IAppDomainAdapter AppDomain()
        {
            return Instance().CreateAppDomain();
        }

        public static ITypeAdapter Type(Type type)
        {
            return Instance().CreateType(type);
        }
        
        public static ITypeBuilderAdapter TypeBuilder(TypeBuilder type)
        {
            return Instance().CreateTypeBuilder(type);
        }
        
        public static IEnumBuilderAdapter EnumBuilder(EnumBuilder builder)
        {
            return Instance().CreateEnumBuilder(builder);
        }

        public static IMethodInfoAdapter MethodInfo(MethodInfo info)
        {
            return Instance().CreateMethodInfo(info);
        }
        
        public static IMethodBuilderAdapter MethodBuilder(MethodBuilder builder)
        {
            return Instance().CreateMethodBuilder(builder);
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
        
        public static IConstructorBuilderAdapter ConstructorBuilder(ConstructorBuilder builder)
        {
            return Instance().CreateConstructorBuilder(builder);
        }

        public static IParameterBuilderAdapter ParameterBuilder(ParameterBuilder builder)
        {
            return Instance().CreateParameterBuilder(builder);
        }
        
        public static IModuleBuilderAdapter ModuleBuilder(ModuleBuilder builder)
        {
            return Instance().CreateModuleBuilder(builder);
        }
        
        public static IParameterInfoAdapter ParameterInfo(ParameterInfo info)
        {
            return Instance().CreateParameterInfo(info);
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

        public static IILGeneratorAdapter ILGenerator(ILGenerator generator)
        {
            return Instance().CreateILGenerator(generator);
        }
        
        public static AssemblyAdapter Assembly(Assembly assembly)
        {
            return Instance().CreateAssembly(assembly);
        }
        
        public static IModuleAdapter Module(Module generator)
        {
            return Instance().CreateModule(generator);
        }
        
        public static IGenericTypeParameterBuilderAdapter GenericTypeParameterBuilder(GenericTypeParameterBuilder builder)
        {
            return Instance().CreateGenericTypeParameterBuilder(builder);
        }

        public static IAssemblyBuilderAdapter AssemblyBuilder(string assemblyName, string path)
        {
            return Instance().CreateAssemblyBuilder(assemblyName, path);
        }



        protected abstract IAppDomainAdapter CreateAppDomain();
        protected abstract ITypeAdapter CreateType(Type type);
        protected abstract ITypeBuilderAdapter CreateTypeBuilder(TypeBuilder builder);
        protected abstract IEnumBuilderAdapter CreateEnumBuilder(EnumBuilder builder);
        protected abstract IMethodInfoAdapter CreateMethodInfo(MethodInfo info);
        protected abstract IMethodBuilderAdapter CreateMethodBuilder(MethodBuilder builder);
        protected abstract IFieldInfoAdapter CreateFieldInfo(FieldInfo info);
        protected abstract ILocalBuilderAdapter CreateLocalBuilder(LocalBuilder builder);
        protected abstract IFieldBuilderAdapter CreateFieldBuilder(FieldBuilder builder);
        protected abstract IParameterBuilderAdapter CreateParameterBuilder(ParameterBuilder builder);
        protected abstract IParameterInfoAdapter CreateParameterInfo(ParameterInfo info);
        protected abstract IEventBuilderAdapter CreateEventBuilder(EventBuilder builder);
        protected abstract IModuleBuilderAdapter CreateModuleBuilder(ModuleBuilder builder);
        protected abstract IAssemblyBuilderAdapter CreateAssemblyBuilder(AssemblyBuilder builder);
        protected abstract IGenericTypeParameterBuilderAdapter CreateGenericTypeParameterBuilder(GenericTypeParameterBuilder builder);
        protected abstract ICustomAttributeBuilderAdapter CreateCustomAttributeBuilder(IConstructorInfoAdapter constructor, object[] args);
        protected abstract ICustomAttributeBuilderAdapter CreateCustomAttributeBuilder(IConstructorInfoAdapter constructor, object[] constructorArgs, IPropertyInfoAdapter[] namedProperties, object[] propertyValues);
        protected abstract ICustomAttributeBuilderAdapter CreateCustomAttributeBuilder(
            IConstructorInfoAdapter constructor,
            object[] constructorArgs, IPropertyInfoAdapter[] namedProperties, object[] propertyValues,
            IFieldInfoAdapter[] fields, object[] fieldValues);
        protected abstract IPropertyInfoAdapter CreatePropertyInfo(PropertyInfo info);
        protected abstract IPropertyBuilderAdapter CreatePropertyBuilder(PropertyBuilder builder);
        protected abstract IConstructorInfoAdapter CreateConstructorInfo(ConstructorInfo info);
        protected abstract IConstructorBuilderAdapter CreateConstructorBuilder(ConstructorBuilder builder);
        protected abstract IMemberInfoAdapter CreateMemberInfo(MemberInfo info);
        protected abstract IILGeneratorAdapter CreateILGenerator(ILGenerator generator);
        protected abstract AssemblyAdapter CreateAssembly(Assembly assembly);
        protected abstract IModuleAdapter CreateModule(Module module);
        protected abstract IAssemblyBuilderAdapter CreateAssemblyBuilder(string assemblyName, string path);


        private static AdapterFactory Instance()
        {
            if (!(_instance is object))
            {
#if !NETCOREAPP
                _instance = new FrameworkAdapterFactory();
                System.Console.WriteLine("Using framework");
#else
                _instance =  new RoslynAdapterFactory();
                Console.WriteLine("Using roslyn");
#endif
            }

            return _instance;

        }
    }

    public static class AdapterExtensions
    {
        private static Dictionary<object, IAdapter> _adaptees = new Dictionary<object, IAdapter>(new AdapterReferenceEqualityComparer());

        public static void Init()
        {
            _adaptees.Clear();
        }

        public static ITypeAdapter GetAdapter(this Type type)
        {
            if (type is null)
                return null;

            if (_adaptees.TryGetValue(type, out var adaptee))
                return adaptee as ITypeAdapter;

            var typeName = type.GetType().Name.Replace("Runtime", "");
            var method = typeof(AdapterFactory).GetMethod(typeName, BindingFlags.Static | BindingFlags.Public) ?? typeof(AdapterFactory).GetMethod("Type", BindingFlags.Static | BindingFlags.Public);
            var instance = method.Invoke(null, new [] { type }) as ITypeAdapter;
            
            _adaptees.Add(type, instance);
            return instance;
        }

        public static ITypeBuilderAdapter GetAdapter(this TypeBuilder builder)
        {
            if (builder is null)
                return null;
            
            if (_adaptees.TryGetValue(builder, out var adaptee)) 
                return adaptee as ITypeBuilderAdapter;
            
            var instance = AdapterFactory.TypeBuilder(builder);
            _adaptees.Add(builder, instance);
            return instance;
        }

        public static IEnumBuilderAdapter GetAdapter(this EnumBuilder builder)
        {
            if (builder is null)
                return null;
            
            if (_adaptees.TryGetValue(builder, out var adaptee)) 
                return adaptee as IEnumBuilderAdapter;
            
            var instance = AdapterFactory.EnumBuilder(builder);
            _adaptees.Add(builder, instance);
            return instance;
        }

        public static IConstructorInfoAdapter GetAdapter(this ConstructorInfo info)
        {
            if (info is null)
                return null;

            if (_adaptees.TryGetValue(info, out var adaptee)) 
                return adaptee as IConstructorInfoAdapter;
            
            var typeName = info.GetType().Name.Replace("Runtime", "");
            var method = typeof(AdapterFactory).GetMethod(typeName, BindingFlags.Static | BindingFlags.Public) ?? typeof(AdapterFactory).GetMethod("ConstructorInfo", BindingFlags.Static | BindingFlags.Public);
            var instance = method.Invoke(null, new [] { info }) as IConstructorInfoAdapter;
                
            _adaptees.Add(info, instance);
            return instance;
        }
        
        public static IConstructorBuilderAdapter GetAdapter(this ConstructorBuilder builder)
        {
            if (builder is null)
                return null;
            
            if (_adaptees.TryGetValue(builder, out var adaptee))
                return adaptee as IConstructorBuilderAdapter;

            var instance = AdapterFactory.ConstructorBuilder(builder);
            _adaptees.Add(builder, instance);
            return instance;
        }

        public static IMethodInfoAdapter GetAdapter(this MethodInfo info)
        {
            if (info is null)
                return null;
            
            if (_adaptees.TryGetValue(info, out var adaptee)) 
                return adaptee as IMethodInfoAdapter;

            var typeName = info.GetType().Name.Replace("Runtime", "");
            var method = typeof(AdapterFactory).GetMethod(typeName, BindingFlags.Static | BindingFlags.Public) ?? typeof(AdapterFactory).GetMethod("MethodInfo", BindingFlags.Static | BindingFlags.Public);
            var instance = method.Invoke(null, new [] { info }) as IMethodInfoAdapter;

            _adaptees.Add(info, instance);
            return instance;
        }
        
        public static IMethodBuilderAdapter GetAdapter(this MethodBuilder builder)
        {
            if (builder is null)
                return null;
            
            if (_adaptees.TryGetValue(builder, out var adaptee))
                return adaptee as IMethodBuilderAdapter;

            var instance = AdapterFactory.MethodBuilder(builder);
            _adaptees.Add(builder, instance);
            return instance;
        }

        public static IFieldInfoAdapter GetAdapter(this FieldInfo info)
        {
            if (info is null)
                return null;
            
            if (_adaptees.TryGetValue(info, out var adaptee)) 
                return adaptee as IFieldInfoAdapter;
            
            var typeName = info.GetType().Name.Replace("Runtime", "");
            var method = typeof(AdapterFactory).GetMethod(typeName, BindingFlags.Static | BindingFlags.Public) ?? typeof(AdapterFactory).GetMethod("FieldInfo", BindingFlags.Static | BindingFlags.Public);
            var instance = method.Invoke(null, new [] { info }) as IFieldInfoAdapter;

            _adaptees.Add(info, instance);
            return instance;
        }

        public static IPropertyInfoAdapter GetAdapter(this PropertyInfo info)
        {
            if (info is null)
                return null;
            
            if (_adaptees.TryGetValue(info, out var adaptee)) 
                return adaptee as IPropertyInfoAdapter;
            
            var typeName = info.GetType().Name.Replace("Runtime", "");
            var method = typeof(AdapterFactory).GetMethod(typeName, BindingFlags.Static | BindingFlags.Public) ?? typeof(AdapterFactory).GetMethod("PropertyInfo", BindingFlags.Static | BindingFlags.Public);
            var instance = method.Invoke(null, new [] { info }) as IPropertyInfoAdapter;

            _adaptees.Add(info, instance);
            return instance;
        }
        
        public static IMemberInfoAdapter GetAdapter(this MemberInfo info)
        {
            if (info is null)
                return null;
            
            if (_adaptees.TryGetValue(info, out var adaptee)) 
                return adaptee as IMemberInfoAdapter;
            
            var typeName = info.GetType().Name.Replace("Runtime", "");
            var method = typeof(AdapterFactory).GetMethod(typeName, BindingFlags.Static | BindingFlags.Public) ?? typeof(AdapterFactory).GetMethod("MemberInfo", BindingFlags.Static | BindingFlags.Public);
            var instance = method.Invoke(null, new [] { info }) as IMemberInfoAdapter;
            
            _adaptees.Add(info, instance);
            return instance;
        }

        public static IParameterBuilderAdapter GetAdapter(this ParameterBuilder builder)
        {
            if (builder is null)
                return null;
            
            if (_adaptees.TryGetValue(builder, out var adaptee))
                return adaptee as IParameterBuilderAdapter;

            var instance = AdapterFactory.ParameterBuilder(builder);
            _adaptees.Add(builder, instance);
            return instance;
        }
        
        public static IParameterInfoAdapter GetAdapter(this ParameterInfo info)
        {
            if (info is null)
                return null;
            
            if (_adaptees.TryGetValue(info, out var adaptee))
                return adaptee as IParameterInfoAdapter;

            var instance = AdapterFactory.ParameterInfo(info);
            _adaptees.Add(info, instance);
            return instance;
        }

        public static IEventBuilderAdapter GetAdapter(this EventBuilder builder)
        {
            if (builder is null)
                return null;
            
            if (_adaptees.TryGetValue(builder, out var adaptee))
                return adaptee as IEventBuilderAdapter;
            
            var instance = AdapterFactory.EventBuilder(builder);
            _adaptees.Add(builder, instance);
            return instance;
        }
        
        public static IGenericTypeParameterBuilderAdapter GetAdapter(this GenericTypeParameterBuilder builder)
        {
            if (builder is null)
                return null;
            
            if (_adaptees.TryGetValue(builder, out var adaptee))
                return adaptee as IGenericTypeParameterBuilderAdapter;

            var instance = AdapterFactory.GenericTypeParameterBuilder(builder);
            _adaptees.Add(builder, instance);
            return instance;
        }
        
        public static IAssemblyBuilderAdapter GetAdapter(this AssemblyBuilder builder)
        {
            if (builder is null)
                return null;
            
            if (_adaptees.TryGetValue(builder, out var adaptee))
                return adaptee as IAssemblyBuilderAdapter;

            var instance = AdapterFactory.AssemblyBuilder(builder);
            _adaptees.Add(builder, instance);
            return instance;
        }
        
        public static IModuleBuilderAdapter GetAdapter(this ModuleBuilder builder)
        {
            if (builder is null)
                return null;
            
            if (_adaptees.TryGetValue(builder, out var adaptee))
                return adaptee as IModuleBuilderAdapter;

            var instance = AdapterFactory.ModuleBuilder(builder);
            _adaptees.Add(builder, instance);
            return instance;
        }

        public static IILGeneratorAdapter GetAdapter(this ILGenerator generator)
        {
            if (generator is null)
                return null;
            
            if (_adaptees.TryGetValue(generator, out var adaptee))
                return adaptee as IILGeneratorAdapter;

            var instance = AdapterFactory.ILGenerator(generator);
            _adaptees.Add(generator, instance);
            return instance;
        }
        
        public static AssemblyAdapter GetAdapter(this Assembly assembly)
        {
            if (assembly is null)
                return null;
            
            if (_adaptees.TryGetValue(assembly, out var adaptee))
                return adaptee as AssemblyAdapter;

            var instance = AdapterFactory.Assembly(assembly);
            _adaptees.Add(assembly, instance);
            return instance;
        }
        
        public static IModuleAdapter GetAdapter(this Module module)
        {
            if (module is null)
                return null;
            
            if (_adaptees.TryGetValue(module, out var adaptee))
                return adaptee as IModuleAdapter;

            var typeName = module.GetType().Name.Replace("Runtime", "");
            var method = typeof(AdapterFactory).GetMethod(typeName, BindingFlags.Static | BindingFlags.Public) ?? typeof(AdapterFactory).GetMethod("Module", BindingFlags.Static | BindingFlags.Public);
            var instance = method.Invoke(null, new [] { module }) as IModuleAdapter;
            
            _adaptees.Add(module, instance);
            return instance;
        }
        
        public static IFieldBuilderAdapter GetAdapter(this FieldBuilder builder)
        {
            if (builder is null)
                return null;
            
            if (_adaptees.TryGetValue(builder, out var adaptee))
                return adaptee as IFieldBuilderAdapter;

            var instance = AdapterFactory.FieldBuilder(builder);
            _adaptees.Add(builder, instance);
            return instance;
        }

        public static IPropertyBuilderAdapter GetAdapter(this PropertyBuilder builder)
        {
            if (builder is null)
                return null;
            
            if (_adaptees.TryGetValue(builder, out var adaptee))
                return adaptee as IPropertyBuilderAdapter;

            var instance = AdapterFactory.PropertyBuilder(builder);
            _adaptees.Add(builder, instance);
            return instance;
        }
        
        public static ILocalBuilderAdapter GetAdapter(this LocalBuilder builder)
        {
            if (builder is null)
                return null;
            
            if (_adaptees.TryGetValue(builder, out var adaptee))
                return adaptee as ILocalBuilderAdapter;

            var instance = AdapterFactory.LocalBuilder(builder);
            _adaptees.Add(builder, instance);
            return instance;
        }
    }

    public class AdapterReferenceEqualityComparer : IEqualityComparer<object>
    {
        public bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return 0;
        }
    }
}