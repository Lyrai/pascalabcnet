using System;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IModuleBuilderAdapter
    {
        ISymbolDocumentWriter DefineDocument(string url, Guid language, Guid languageVendor, Guid documentType);
        TypeBuilderAdapter DefineType(string name, TypeAttributes attr, TypeAdapter parent);
        TypeBuilderAdapter DefineType(string name, TypeAttributes attr);
        TypeBuilderAdapter DefineType(string name, TypeAttributes attr, TypeAdapter parent, TypeAdapter[] types);
        EnumBuilderAdapter DefineEnum(string name, TypeAttributes attr, TypeAdapter parent);
        IMethodInfoAdapter GetArrayMethod (TypeAdapter arrayClass, string methodName, CallingConventions callingConvention, TypeAdapter returnType, TypeAdapter[] parameterTypes);
        void DefineManifestResource(string filename, FileStream stream, ResourceAttributes attributes);
        TypeAdapter GetType(string name);
    }
}