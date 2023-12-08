using System;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public abstract class TypeAdapter
    {
        public abstract bool IsGenericType { get; }
        public abstract bool IsArray { get; }
        public abstract bool IsGenericTypeDefinition { get; }
        public abstract bool IsGenericParameter { get; }
        public abstract bool IsValueType { get; }
        public abstract bool IsPointer { get; }
        public abstract bool IsEnum { get; }
        public abstract bool IsInterface { get; }
        public abstract bool IsClass { get; }
        public abstract bool IsPrimitive { get; }
        public abstract bool IsSealed { get; }
        public abstract bool IsAbstract { get; }
        public abstract bool IsByRef { get; }
        public abstract bool IsNotPublic { get; }
        public abstract IMethodInfoAdapter DeclaringMethod { get; }
        public abstract string FullName { get; }
        public abstract string Name { get; }
        public abstract string Namespace { get; }
        public abstract string AssemblyQualifiedName { get; }
        public abstract AssemblyAdapter Assembly { get; }
        public abstract int GenericParameterPosition { get; }
        public abstract TypeAdapter BaseType { get; }
        public abstract TypeAdapter DeclaringType { get; }
        public abstract IModuleAdapter Module { get; }
        public static TypeAdapter[] EmptyTypes => new TypeAdapter[] { };

        public abstract IMethodInfoAdapter GetMethod(string name);
        public abstract IMethodInfoAdapter GetMethod(string name, TypeAdapter[] parameterTypes);
        public abstract IMethodInfoAdapter GetMethod(string name, BindingFlags flags);
        public abstract IMethodInfoAdapter[] GetMethods();
        public abstract IMethodInfoAdapter[] GetMethods(BindingFlags flags);
        public abstract IConstructorInfoAdapter GetConstructor(TypeAdapter[] parameterTypes);
        public abstract IConstructorInfoAdapter[] GetConstructors();
        public abstract IConstructorInfoAdapter[] GetConstructors(BindingFlags flags);
        public abstract TypeAdapter[] GetGenericArguments();
        public abstract TypeAdapter GetElementType();
        public abstract TypeAdapter GetGenericTypeDefinition();
        public abstract IPropertyInfoAdapter GetProperty(string name);
        public abstract IPropertyInfoAdapter GetProperty(string name, BindingFlags flags);
        public abstract IFieldInfoAdapter GetField(string name, BindingFlags flags);
        public abstract TypeAdapter GetInterface(string name);
        public abstract TypeAdapter[] GetInterfaces();
        public abstract TypeAdapter[] GetNestedTypes();
        public abstract IMemberInfoAdapter[] GetMember(string name, BindingFlags flags);
        public abstract IFieldInfoAdapter[] GetFields();
        public abstract IMemberInfoAdapter[] GetDefaultMembers();
        public abstract IMemberInfoAdapter[] GetMembers(BindingFlags flags);
        public abstract int GetArrayRank();
        public abstract TypeAdapter MakeGenericType(TypeAdapter type);
        public abstract TypeAdapter MakeGenericType(TypeAdapter[] types);
        public abstract TypeAdapter MakeArrayType();
        public abstract TypeAdapter MakeArrayType(int rank);
        public abstract TypeAdapter MakePointerType();
        public abstract TypeAdapter MakeByRefType();
        public abstract object[] GetCustomAttributes(TypeAdapter attributeType, bool inherit);
        public abstract object[] GetCustomAttributes(bool inherit);

        public static TypeCode GetTypeCode(TypeAdapter type)
        {
            return TypeCode.Boolean;
        }

        public static TypeAdapter GetType(string name)
        {
            return null;
        }
        
        /*public static implicit operator TypeAdapter(Type type)
        {
            return type.GetAdapter();
        }*/

        public static bool operator ==(TypeAdapter adapter, Type type)
        {
            return adapter.Equals(type.GetAdapter());
        }

        public static bool operator !=(TypeAdapter adapter, Type type)
        {
            return !(adapter == type);
        }
        
        public static bool operator ==(Type type, TypeAdapter adapter)
        {
            return adapter.Equals(type.GetAdapter());
        }

        public static bool operator !=(Type type, TypeAdapter adapter)
        {
            return !(adapter == type);
        }
        
        public static bool operator ==(TypeAdapter adapter, TypeAdapter type)
        {
            return adapter.Equals(type);
        }

        public static bool operator !=(TypeAdapter adapter, TypeAdapter type)
        {
            return !(adapter == type);
        }
        
        public abstract bool Equals(TypeAdapter other);
    }
}