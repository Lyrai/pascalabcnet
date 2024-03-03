using System;
using System.Collections.Immutable;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Roslyn.Utilities;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynModuleBuilderAdapter: RoslynModuleAdapter, IModuleBuilderAdapter
    {
        public RoslynModuleBuilderAdapter(AssemblyAdapter assembly) : base(assembly)
        { }
        
        public ISymbolDocumentWriter DefineDocument(string url, Guid language, Guid languageVendor, Guid documentType)
        {
            throw new NotImplementedException();
        }

        public ITypeBuilderAdapter DefineType(string name, TypeAttributes attr, ITypeAdapter parent)
        {
            return DefineType(name, attr, parent, null);
        }

        public ITypeBuilderAdapter DefineType(string name, TypeAttributes attr)
        {
            return DefineType(name, attr, null);
        }

        public ITypeBuilderAdapter DefineType(string name, TypeAttributes attr, ITypeAdapter parent, ITypeAdapter[] interfaces)
        {
            if (_members.ContainsKey(name))
            {
                throw new InvalidOperationException("Type exists");
            }

            var type = new RoslynTypeBuilderAdapter(this, name, attr, parent, interfaces);
            _members.Add(name, type);

            return type;
        }

        public IEnumBuilderAdapter DefineEnum(string name, TypeAttributes attr, ITypeAdapter parent)
        {
            if (_members.ContainsKey(name))
            {
                throw new InvalidOperationException("Type exists");
            }
            
            var type = new RoslynEnumBuilderAdapter(this, name, attr, parent);
            _members.Add(name, type);

            return type;
        }

        public IMethodInfoAdapter GetArrayMethod(ITypeAdapter arrayClass, string methodName, CallingConventions callingConvention,
            ITypeAdapter returnType, ITypeAdapter[] parameterTypes)
        {
            throw new NotImplementedException();
        }

        public void DefineManifestResource(string filename, FileStream stream, ResourceAttributes attributes)
        {
            throw new NotImplementedException();
        }
    }
}