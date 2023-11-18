using System;
using System.Diagnostics.SymbolStore;
using System.Reflection;

namespace NETGenerator.Adapters
{
    public interface IModuleBuilderAdapter
    {
        ISymbolDocumentWriter DefineDocument(string url, Guid language, Guid languageVendor, Guid documentType);
        ITypeBuilderAdapter DefineType(string name, TypeAttributes attr, Type parent);
        ITypeBuilderAdapter DefineType (string name, TypeAttributes attr);
    }
}