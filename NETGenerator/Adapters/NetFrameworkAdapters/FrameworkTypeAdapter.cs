using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkTypeAdapter: ITypeAdapter
    {
        public bool IsGenericType => Adaptee.IsGenericType;
        public bool IsArray => Adaptee.IsArray;
        public bool IsGenericTypeDefinition => Adaptee.IsGenericTypeDefinition;
        public bool IsGenericParameter => Adaptee.IsGenericParameter;
        public bool IsValueType => Adaptee.IsValueType;
        public bool IsPointer => Adaptee.IsPointer;
        public bool IsEnum => Adaptee.IsEnum;
        public bool IsInterface => Adaptee.IsInterface;
        public bool IsClass => Adaptee.IsClass;
        public bool IsPrimitive => Adaptee.IsPrimitive;
        public bool IsSealed => Adaptee.IsSealed;
        public bool IsAbstract => Adaptee.IsAbstract;
        public bool IsByRef => Adaptee.IsByRef;
        public bool IsNotPublic => Adaptee.IsNotPublic;
        public IMethodInfoAdapter DeclaringMethod => (Adaptee.DeclaringMethod as MethodInfo).GetAdapter();
        public string FullName => Adaptee.FullName;
        public string Name => Adaptee.Name;
        public string Namespace => Adaptee.Namespace;
        public string AssemblyQualifiedName => Adaptee.AssemblyQualifiedName;
        public AssemblyAdapter Assembly => Adaptee.Assembly.GetAdapter();
        public int GenericParameterPosition => Adaptee.GenericParameterPosition;
        public ITypeAdapter BaseType => Adaptee.BaseType.GetAdapter();
        public ITypeAdapter DeclaringType => Adaptee.DeclaringType.GetAdapter();
        public IModuleAdapter Module => Adaptee.Module.GetAdapter();
        public TypeAttributes Attributes => Adaptee.Attributes;
        public IEnumerable<ITypeAdapter> ImplementedInterfaces => Enumerable.Empty<ITypeAdapter>();

        public Type Adaptee { get; }

        public FrameworkTypeAdapter(Type adaptee)
        {
            Adaptee = adaptee;
        }
        
        public IMethodInfoAdapter GetMethod(string name)
        {
            return Adaptee.GetMethod(name).GetAdapter();
        }

        public IMethodInfoAdapter GetMethod(string name, ITypeAdapter[] parameterTypes)
        {
            return Adaptee.GetMethod(name, GetAdaptee(parameterTypes)).GetAdapter();
        }

        public IMethodInfoAdapter GetMethod(string name, BindingFlags flags)
        {
            return Adaptee.GetMethod(name, flags).GetAdapter();
        }

        public IMethodInfoAdapter[] GetMethods()
        {
            return Adaptee.GetMethods().Select(m => m.GetAdapter()).ToArray();
        }

        public IMethodInfoAdapter[] GetMethods(BindingFlags flags)
        {
            return Adaptee.GetMethods().Select(m => m.GetAdapter()).ToArray();
        }

        public IConstructorInfoAdapter GetConstructor(ITypeAdapter[] parameterTypes)
        {
            return Adaptee.GetConstructor(GetAdaptee(parameterTypes)).GetAdapter();
        }

        public IConstructorInfoAdapter[] GetConstructors()
        {
            return Adaptee.GetConstructors().Select(c => c.GetAdapter()).ToArray();
        }

        public IConstructorInfoAdapter[] GetConstructors(BindingFlags flags)
        {
            return Adaptee.GetConstructors(flags).Select(c => c.GetAdapter()).ToArray();
        }

        public ITypeAdapter[] GetGenericArguments()
        {
            return Adaptee.GetGenericArguments().Select(t => t.GetAdapter()).ToArray();
        }

        public ITypeAdapter GetElementType()
        {
            return Adaptee.GetElementType().GetAdapter();
        }

        public ITypeAdapter GetGenericTypeDefinition()
        {
            return Adaptee.GetGenericTypeDefinition().GetAdapter();
        }

        public IPropertyInfoAdapter GetProperty(string name)
        {
            return Adaptee.GetProperty(name).GetAdapter();
        }

        public IPropertyInfoAdapter GetProperty(string name, BindingFlags flags)
        {
            return Adaptee.GetProperty(name, flags).GetAdapter();
        }

        public IPropertyInfoAdapter[] GetProperties()
        {
            return Adaptee.GetProperties().Select(property => property.GetAdapter()).ToArray();
        }

        public IFieldInfoAdapter GetField(string name, BindingFlags flags)
        {
            return Adaptee.GetField(name, flags).GetAdapter();
        }

        public ITypeAdapter GetInterface(string name)
        {
            return Adaptee.GetInterface(name).GetAdapter();
        }

        public ITypeAdapter[] GetInterfaces()
        {
            return Adaptee.GetInterfaces().Select(t => t.GetAdapter()).ToArray();
        }

        public ITypeAdapter[] GetNestedTypes()
        {
            return Adaptee.GetNestedTypes().Select(t => t.GetAdapter()).ToArray();
        }

        public IMemberInfoAdapter[] GetMember(string name, BindingFlags flags)
        {
            return Adaptee.GetMember(name, flags).Select(t => t.GetAdapter()).ToArray();
        }

        public IFieldInfoAdapter[] GetFields()
        {
            return Adaptee.GetFields().Select(t => t.GetAdapter()).ToArray();
        }

        public IMemberInfoAdapter[] GetDefaultMembers()
        {
            return Adaptee.GetDefaultMembers().Select(t => t.GetAdapter()).ToArray();
        }

        public IMemberInfoAdapter[] GetMembers(BindingFlags flags)
        {
            return Adaptee.GetMembers(flags).Select(t => t.GetAdapter()).ToArray();
        }

        public int GetArrayRank()
        {
            return Adaptee.GetArrayRank();
        }

        public ITypeAdapter MakeGenericType(params ITypeAdapter[] types)
        {
            return Adaptee.MakeGenericType(GetAdaptee(types)).GetAdapter();
        }

        public ITypeAdapter MakeArrayType()
        {
            return Adaptee.MakeArrayType().GetAdapter();
        }

        public ITypeAdapter MakeArrayType(int rank)
        {
            return Adaptee.MakeArrayType(rank).GetAdapter();
        }

        public ITypeAdapter MakePointerType()
        {
            return Adaptee.MakePointerType().GetAdapter();
        }

        public ITypeAdapter MakeByRefType()
        {
            return Adaptee.MakeByRefType().GetAdapter();
        }

        public object[] GetCustomAttributes(ITypeAdapter attributeType, bool inherit)
        {
            return Adaptee.GetCustomAttributes(GetAdaptee(attributeType), inherit);
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            return Adaptee.GetCustomAttributes(inherit);
        }

        public bool Equals(ITypeAdapter other)
        {
            return Adaptee == GetAdaptee(other);
        }

        public override bool Equals(object other)
        {
            if (other is ITypeAdapter adapter)
            {
                return Equals(adapter);
            }

            if (other is Type t)
            {
                return Adaptee == t;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Adaptee.GetHashCode();
        }

        public static bool operator ==(FrameworkTypeAdapter first, ITypeAdapter second)
        {
            return first?.Adaptee == (second as FrameworkTypeAdapter)?.Adaptee;
        }

        public static bool operator !=(FrameworkTypeAdapter first, ITypeAdapter second)
        {
            return !(first == second);
        }

        private Type GetAdaptee(ITypeAdapter adapter)
        {
            return (adapter as FrameworkTypeAdapter)?.Adaptee;
        }

        private Type[] GetAdaptee(ITypeAdapter[] adapters)
        {
            return adapters.Select(t => (t as FrameworkTypeAdapter)?.Adaptee).ToArray();
        }
    }
}