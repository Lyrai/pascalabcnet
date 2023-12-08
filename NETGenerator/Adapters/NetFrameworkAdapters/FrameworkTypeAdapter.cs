using System;
using System.Linq;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkTypeAdapter: TypeAdapter
    {
        public override bool IsGenericType { get; }
        public override bool IsArray { get; }
        public override bool IsGenericTypeDefinition { get; }
        public override bool IsGenericParameter { get; }
        public override bool IsValueType { get; }
        public override bool IsPointer { get; }
        public override bool IsEnum { get; }
        public override bool IsInterface { get; }
        public override bool IsClass { get; }
        public override bool IsPrimitive { get; }
        public override string FullName { get; }
        public override string Name { get; }
        public override int GenericParameterPosition { get; }
        public override TypeAdapter BaseType { get; }
        public override TypeAdapter DeclaringType { get; }
        public override IModuleAdapter Module { get; }
        public Type Adaptee { get; }

        public FrameworkTypeAdapter(Type adaptee)
        {
            Adaptee = adaptee;
        }
        
        public override IMethodInfoAdapter GetMethod(string name)
        {
            return Adaptee.GetMethod(name).GetAdapter();
        }

        public override IMethodInfoAdapter GetMethod(string name, TypeAdapter[] parameterTypes)
        {
            return Adaptee.GetMethod(name, GetAdaptee(parameterTypes)).GetAdapter();
        }

        public override IMethodInfoAdapter GetMethod(string name, BindingFlags flags)
        {
            return Adaptee.GetMethod(name, flags).GetAdapter();
        }

        public override IMethodInfoAdapter[] GetMethods()
        {
            return Adaptee.GetMethods().Select(m => m.GetAdapter()).ToArray();
        }

        public override IMethodInfoAdapter[] GetMethods(BindingFlags flags)
        {
            return Adaptee.GetMethods().Select(m => m.GetAdapter()).ToArray();
        }

        public override IConstructorInfoAdapter GetConstructor(TypeAdapter[] parameterTypes)
        {
            return Adaptee.GetConstructor(GetAdaptee(parameterTypes)).GetAdapter();
        }

        public override IConstructorInfoAdapter[] GetConstructors()
        {
            return Adaptee.GetConstructors().Select(c => c.GetAdapter()).ToArray();
        }

        public override TypeAdapter[] GetGenericArguments()
        {
            return Adaptee.GetGenericArguments().Select(t => t.GetAdapter()).ToArray();
        }

        public override TypeAdapter GetElementType()
        {
            return Adaptee.GetElementType().GetAdapter();
        }

        public override TypeAdapter GetGenericTypeDefinition()
        {
            return Adaptee.GetGenericTypeDefinition().GetAdapter();
        }

        public override IPropertyInfoAdapter GetProperty(string name)
        {
            return Adaptee.GetProperty(name).GetAdapter();
        }

        public override IPropertyInfoAdapter GetProperty(string name, BindingFlags flags)
        {
            return Adaptee.GetProperty(name, flags).GetAdapter();
        }

        public override TypeAdapter GetInterface(string name)
        {
            return Adaptee.GetInterface(name).GetAdapter();
        }

        public override TypeAdapter[] GetInterfaces()
        {
            throw new System.NotImplementedException();
        }

        public override IMemberInfoAdapter[] GetMember(string name, BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public override int GetArrayRank()
        {
            throw new System.NotImplementedException();
        }

        public override TypeAdapter MakeGenericType(TypeAdapter type)
        {
            throw new System.NotImplementedException();
        }

        public override TypeAdapter MakeGenericType(TypeAdapter[] types)
        {
            throw new System.NotImplementedException();
        }

        public override TypeAdapter MakeArrayType()
        {
            throw new System.NotImplementedException();
        }

        public override TypeAdapter MakeArrayType(int rank)
        {
            throw new System.NotImplementedException();
        }

        public override TypeAdapter MakePointerType()
        {
            throw new System.NotImplementedException();
        }

        public override TypeAdapter MakeByRefType()
        {
            throw new System.NotImplementedException();
        }

        public override bool Equals(TypeAdapter other)
        {
            throw new System.NotImplementedException();
        }

        private Type GetAdaptee(TypeAdapter adapter)
        {
            return (adapter as FrameworkTypeAdapter).Adaptee;
        }

        private Type[] GetAdaptee(TypeAdapter[] adapters)
        {
            return adapters.Select(t => (t as FrameworkTypeAdapter).Adaptee).ToArray();
        }
    }
}