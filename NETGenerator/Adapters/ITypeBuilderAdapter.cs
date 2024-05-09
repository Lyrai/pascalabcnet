using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface ITypeBuilderAdapter: ITypeAdapter
    {
        IConstructorBuilderAdapter DefineConstructor(MethodAttributes attributes, CallingConventions conventions, ITypeAdapter[] parameterTypes);
        IFieldBuilderAdapter DefineField(string name, ITypeAdapter type, FieldAttributes attributes);
        IConstructorBuilderAdapter DefineDefaultConstructor(MethodAttributes attributes);
        IMethodBuilderAdapter DefineMethod(string name, MethodAttributes attributes, ITypeAdapter returnType, ITypeAdapter[] parameterTypes);
        IMethodBuilderAdapter DefineMethod(string name, MethodAttributes attributes);
        IGenericTypeParameterBuilderAdapter[] DefineGenericParameters(string[] names);
        IEventBuilderAdapter DefineEvent(string name, EventAttributes attributes, ITypeAdapter type);
        IPropertyBuilderAdapter DefineProperty(string name, PropertyAttributes attributes, ITypeAdapter returnType, ITypeAdapter[] types);
        IMethodBuilderAdapter DefinePInvokeMethod(string name, string dllName, string entryName, MethodAttributes attributes, CallingConventions callingConvention, ITypeAdapter returnType, ITypeAdapter[] parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet);
        ITypeBuilderAdapter DefineNestedType(string name, TypeAttributes attributes);
        void DefineMethodOverride(IMethodInfoAdapter methodBody, IMethodInfoAdapter methodInfo);
        IConstructorBuilderAdapter DefineTypeInitializer();
        void AddInterfaceImplementation(ITypeAdapter type);
        void SetCustomAttribute(IConstructorInfoAdapter constructor, byte[] attribute);
        void SetCustomAttribute(ICustomAttributeBuilderAdapter constructor);
        void SetParent(ITypeAdapter type);
        ITypeAdapter CreateType();
    }
}