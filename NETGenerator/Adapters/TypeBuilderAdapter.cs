using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public abstract class TypeBuilderAdapter: TypeAdapter
    {
        public static IMethodInfoAdapter GetMethod(TypeAdapter type, IMethodInfoAdapter method)
        {
            return null;
        }

        public static IConstructorInfoAdapter GetConstructor(TypeAdapter type, IConstructorInfoAdapter constructor)
        {
            return null;
        }

        public static IFieldInfoAdapter GetField(TypeAdapter type, IFieldInfoAdapter field)
        {
            return null;
        }

        public abstract IConstructorBuilderAdapter DefineConstructor(MethodAttributes attributes, CallingConventions conventions, TypeAdapter[] parameterTypes);
        public abstract IFieldBuilderAdapter DefineField(string name, TypeAdapter type, FieldAttributes attributes);
        public abstract IConstructorBuilderAdapter DefineDefaultConstructor(MethodAttributes attributes);
        public abstract IMethodBuilderAdapter DefineMethod(string name, MethodAttributes attributes, TypeAdapter returnType, TypeAdapter[] parameterTypes);
        public abstract IMethodBuilderAdapter DefineMethod(string name, MethodAttributes attributes);
        public abstract GenericTypeParameterBuilderAdapter[] DefineGenericParameters(string[] names);
        public abstract IEventBuilderAdapter DefineEvent(string name, EventAttributes attributes, TypeAdapter type);
        public abstract IPropertyBuilderAdapter DefineProperty(string name, PropertyAttributes attributes, TypeAdapter returnType, TypeAdapter[] types);
        public abstract IMethodBuilderAdapter DefinePInvokeMethod(string name, string dllName, string entryName, MethodAttributes attributes, CallingConventions callingConvention, TypeAdapter returnType, TypeAdapter[] parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet);
        public abstract TypeBuilderAdapter DefineNestedType(string name, TypeAttributes attributes);
        public abstract void DefineMethodOverride(IMethodInfoAdapter methodBody, IMethodInfoAdapter methodInfo);
        public abstract IConstructorBuilderAdapter DefineTypeInitializer();
        public abstract void AddInterfaceImplementation(TypeAdapter type);
        public abstract void SetCustomAttribute(IConstructorInfoAdapter constructor, byte[] attribute);
        public abstract void SetCustomAttribute(ICustomAttributeBuilderAdapter constructor);
        public abstract void SetParent(TypeAdapter type);
        public abstract TypeAdapter CreateType();
    }
}