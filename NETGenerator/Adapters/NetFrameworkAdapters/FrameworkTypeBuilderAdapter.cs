using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkTypeBuilderAdapter: FrameworkTypeAdapter, ITypeBuilderAdapter
    {
        public new TypeBuilder Adaptee { get; }

        public FrameworkTypeBuilderAdapter(TypeBuilder adaptee): base(adaptee)
        {
            Adaptee = adaptee;
        }

        public IConstructorBuilderAdapter DefineConstructor(MethodAttributes attributes, CallingConventions conventions,
            ITypeAdapter[] parameterTypes)
        {
            return Adaptee.DefineConstructor(attributes, conventions, GetAdaptee(parameterTypes)).GetAdapter();
        }

        public IFieldBuilderAdapter DefineField(string name, ITypeAdapter type, FieldAttributes attributes)
        {
            return Adaptee.DefineField(name, GetAdaptee(type), attributes).GetAdapter();
        }

        public IConstructorBuilderAdapter DefineDefaultConstructor(MethodAttributes attributes)
        {
            return Adaptee.DefineDefaultConstructor(attributes).GetAdapter();
        }

        public IMethodBuilderAdapter DefineMethod(string name, MethodAttributes attributes, ITypeAdapter returnType,
            ITypeAdapter[] parameterTypes)
        {
            var ts = parameterTypes.Select(t => (t as FrameworkTypeAdapter)?.Adaptee).ToArray();
            return Adaptee.DefineMethod(name, attributes, (returnType as FrameworkTypeAdapter)?.Adaptee, ts).GetAdapter();
        }

        public IMethodBuilderAdapter DefineMethod(string name, MethodAttributes attributes)
        {
            return Adaptee.DefineMethod(name, attributes).GetAdapter();
        }

        public IGenericTypeParameterBuilderAdapter[] DefineGenericParameters(string[] names)
        {
            return Adaptee.DefineGenericParameters(names).Select(p => p.GetAdapter()).ToArray();
        }

        public IEventBuilderAdapter DefineEvent(string name, EventAttributes attributes, ITypeAdapter type)
        {
            return Adaptee.DefineEvent(name, attributes, (type as FrameworkTypeAdapter).Adaptee).GetAdapter();
        }

        public IPropertyBuilderAdapter DefineProperty(string name, PropertyAttributes attributes, ITypeAdapter returnType,
            ITypeAdapter[] types)
        {
            var ts = types.Select(t => (t as FrameworkTypeAdapter)?.Adaptee).ToArray();
            return Adaptee.DefineProperty(name, attributes, (returnType as FrameworkTypeAdapter)?.Adaptee, ts)
                .GetAdapter();
        }

        public IMethodBuilderAdapter DefinePInvokeMethod(string name, string dllName, string entryName, MethodAttributes attributes,
            CallingConventions callingConvention, ITypeAdapter returnType, ITypeAdapter[] parameterTypes,
            CallingConvention nativeCallConv, CharSet nativeCharSet)
        {
            var ts = parameterTypes.Select(t => (t as FrameworkTypeAdapter)?.Adaptee).ToArray();
            return Adaptee.DefinePInvokeMethod(name, dllName, entryName, attributes, callingConvention,
                (returnType as FrameworkTypeAdapter)?.Adaptee, ts, nativeCallConv, nativeCharSet).GetAdapter();
        }

        public ITypeBuilderAdapter DefineNestedType(string name, TypeAttributes attributes)
        {
            return Adaptee.DefineNestedType(name, attributes).GetAdapter();
        }

        public void DefineMethodOverride(IMethodInfoAdapter methodBody, IMethodInfoAdapter methodInfo)
        {
            Adaptee.DefineMethodOverride((methodBody as FrameworkMethodInfoAdapter).Adaptee, (methodInfo as FrameworkMethodInfoAdapter).Adaptee);
        }

        public IConstructorBuilderAdapter DefineTypeInitializer()
        {
            return Adaptee.DefineTypeInitializer().GetAdapter();
        }

        public void AddInterfaceImplementation(ITypeAdapter type)
        {
            Adaptee.AddInterfaceImplementation(GetAdaptee(type));
        }

        public void SetCustomAttribute(IConstructorInfoAdapter constructor, byte[] attribute)
        {
            Adaptee.SetCustomAttribute((constructor as FrameworkConstructorInfoAdapter).Adaptee, attribute);
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter constructor)
        {
            Adaptee.SetCustomAttribute((constructor as FrameworkCustomAttributeBuilderAdapter).Adaptee);
        }

        public void SetParent(ITypeAdapter type)
        {
            Adaptee.SetParent(GetAdaptee(type));
        }

        public ITypeAdapter CreateType()
        {
            return Adaptee.CreateType().GetAdapter();
        }

        private TypeBuilder GetAdaptee(ITypeBuilderAdapter adapter)
        {
            return (adapter as FrameworkTypeBuilderAdapter)?.Adaptee;
        }
        
        private TypeBuilder[] GetAdaptee(ITypeBuilderAdapter[] adapters)
        {
            return adapters.Select(adapter => (adapter as FrameworkTypeBuilderAdapter)?.Adaptee).ToArray();
        }
        
        private Type GetAdaptee(ITypeAdapter adapter)
        {
            return (adapter as FrameworkTypeAdapter)?.Adaptee;
        }
        
        private Type[] GetAdaptee(ITypeAdapter[] adapters)
        {
            return adapters.Select(adapter => (adapter as FrameworkTypeAdapter)?.Adaptee).ToArray();
        }
    }
}