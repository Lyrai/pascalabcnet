using System;
using System.Reflection;

namespace NETGenerator.Adapters
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
        public static TypeAdapter[] EmptyTypes => new TypeAdapter[] { };

        public abstract IMethodInfoAdapter GetMethod(string name);
        public abstract IMethodInfoAdapter GetMethod(string name, TypeAdapter[] parameterTypes);
        public abstract IMethodInfoAdapter GetMethod(string name, BindingFlags flags);
        public abstract IConstructorInfoAdapter GetConstructor(TypeAdapter[] parameterTypes);
        public abstract IConstructorInfoAdapter[] GetConstructors();
        public abstract TypeAdapter[] GetGenericArguments();
        public abstract TypeAdapter GetElementType();
        public abstract TypeAdapter GetGenericTypeDefinition();
        public abstract IPropertyInfoAdapter GetProperty(string name);
        public abstract IPropertyInfoAdapter GetProperty(string name, BindingFlags flags);
        
        
        public static implicit operator TypeAdapter(Type type)
        {
            return AdapterFactory.Type(type);
        }

        public static bool operator ==(TypeAdapter adapter, Type type)
        {
            return adapter.Equals(AdapterFactory.Type(type));
        }

        public static bool operator !=(TypeAdapter adapter, Type type)
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