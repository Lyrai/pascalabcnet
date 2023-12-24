using System;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IModuleBuilderAdapter
    {
        ISymbolDocumentWriter DefineDocument(string url, Guid language, Guid languageVendor, Guid documentType);
        ITypeBuilderAdapter DefineType(string name, TypeAttributes attr, ITypeAdapter parent);
        ITypeBuilderAdapter DefineType(string name, TypeAttributes attr);
        ITypeBuilderAdapter DefineType(string name, TypeAttributes attr, ITypeAdapter parent, ITypeAdapter[] types);
        IEnumBuilderAdapter DefineEnum(string name, TypeAttributes attr, ITypeAdapter parent);
        IMethodInfoAdapter GetArrayMethod (ITypeAdapter arrayClass, string methodName, CallingConventions callingConvention, ITypeAdapter returnType, ITypeAdapter[] parameterTypes);
        void DefineManifestResource(string filename, FileStream stream, ResourceAttributes attributes);
        ITypeAdapter GetType(string name);
    }
}