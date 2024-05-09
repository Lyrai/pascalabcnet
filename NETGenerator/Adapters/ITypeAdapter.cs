using System;
using System.Collections.Generic;
using System.Reflection;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface ITypeAdapter: IAdapter
    {
         bool IsGenericType { get; }
         bool IsArray { get; }
         bool IsGenericTypeDefinition { get; }
         bool IsGenericParameter { get; }
         bool IsValueType { get; }
         bool IsPointer { get; }
         bool IsEnum { get; }
         bool IsInterface { get; }
         bool IsClass { get; }
         bool IsPrimitive { get; }
         bool IsSealed { get; }
         bool IsAbstract { get; }
         bool IsByRef { get; }
         bool IsNotPublic { get; }
         IMethodInfoAdapter DeclaringMethod { get; }
         string FullName { get; }
         string Name { get; }
         string Namespace { get; }
         AssemblyAdapter Assembly { get; }
         int GenericParameterPosition { get; }
         ITypeAdapter BaseType { get; }
         ITypeAdapter DeclaringType { get; }
         IModuleAdapter Module { get; }
         TypeAttributes Attributes { get; }
         IEnumerable<ITypeAdapter> ImplementedInterfaces { get; }

         IMethodInfoAdapter GetMethod(string name);
         IMethodInfoAdapter GetMethod(string name, ITypeAdapter[] parameterTypes);
         IMethodInfoAdapter GetMethod(string name, BindingFlags flags);
         IMethodInfoAdapter[] GetMethods();
         IMethodInfoAdapter[] GetMethods(BindingFlags flags);
         IConstructorInfoAdapter GetConstructor(ITypeAdapter[] parameterTypes);
         IConstructorInfoAdapter[] GetConstructors();
         IConstructorInfoAdapter[] GetConstructors(BindingFlags flags);
         ITypeAdapter[] GetGenericArguments();
         ITypeAdapter GetElementType();
         ITypeAdapter GetGenericTypeDefinition();
         IPropertyInfoAdapter GetProperty(string name);
         IPropertyInfoAdapter GetProperty(string name, BindingFlags flags);
         IFieldInfoAdapter GetField(string name, BindingFlags flags);
         ITypeAdapter GetInterface(string name);
         ITypeAdapter[] GetInterfaces();
         ITypeAdapter[] GetNestedTypes();
         IMemberInfoAdapter[] GetMember(string name, BindingFlags flags);
         IFieldInfoAdapter[] GetFields();
         IMemberInfoAdapter[] GetDefaultMembers();
         IMemberInfoAdapter[] GetMembers(BindingFlags flags);
         int GetArrayRank();
         ITypeAdapter MakeGenericType(params ITypeAdapter[] types);
         ITypeAdapter MakeArrayType();
         ITypeAdapter MakeArrayType(int rank);
         ITypeAdapter MakePointerType();
         ITypeAdapter MakeByRefType();
         object[] GetCustomAttributes(ITypeAdapter attributeType, bool inherit);
         object[] GetCustomAttributes(bool inherit);
    }
}