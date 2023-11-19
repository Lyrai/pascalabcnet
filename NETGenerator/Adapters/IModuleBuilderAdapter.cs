using System;
using System.Diagnostics.SymbolStore;
using System.Reflection;

namespace NETGenerator.Adapters
{
    public interface IModuleBuilderAdapter
    {
        ISymbolDocumentWriter DefineDocument(string url, Guid language, Guid languageVendor, Guid documentType);
        TypeBuilderAdapter DefineType(string name, TypeAttributes attr, Type parent);
        TypeBuilderAdapter DefineType (string name, TypeAttributes attr);
    }
}