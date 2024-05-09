using System;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
#if !NETCOREAPP
    public class FrameworkModuleBuilderAdapter: FrameworkModuleAdapter, IModuleBuilderAdapter
    {
        public ModuleBuilder Adaptee { get; }

        public FrameworkModuleBuilderAdapter(ModuleBuilder builder): base(builder)
        {
            Adaptee = builder;
        }
        
        public ISymbolDocumentWriter DefineDocument(string url, Guid language, Guid languageVendor, Guid documentType)
        {
            return Adaptee.DefineDocument(url, language, languageVendor, documentType);
        }

        public ITypeBuilderAdapter DefineType(string name, TypeAttributes attr, ITypeAdapter parent)
        {
            return Adaptee.DefineType(name, attr, (parent as FrameworkTypeAdapter)?.Adaptee).GetAdapter();
        }

        public ITypeBuilderAdapter DefineType(string name, TypeAttributes attr)
        {
            return Adaptee.DefineType(name, attr).GetAdapter();
        }

        public ITypeBuilderAdapter DefineType(string name, TypeAttributes attr, ITypeAdapter parent, ITypeAdapter[] types)
        {
            var ts = types.Select(t => (t as FrameworkTypeAdapter)?.Adaptee).ToArray();
            return Adaptee.DefineType(name, attr, (parent as FrameworkTypeAdapter)?.Adaptee, ts).GetAdapter();
        }

        public IEnumBuilderAdapter DefineEnum(string name, TypeAttributes attr, ITypeAdapter parent)
        {
            return Adaptee.DefineEnum(name, attr, (parent as FrameworkTypeAdapter).Adaptee).GetAdapter();
        }

        public IMethodInfoAdapter GetArrayMethod(ITypeAdapter arrayClass, string methodName, CallingConventions callingConvention,
            ITypeAdapter returnType, ITypeAdapter[] parameterTypes)
        {
            var ts = parameterTypes.Select(t => (t as FrameworkTypeAdapter)?.Adaptee).ToArray();
            return Adaptee.GetArrayMethod((arrayClass as FrameworkTypeAdapter).Adaptee, methodName, callingConvention,
                (returnType as FrameworkTypeAdapter)?.Adaptee, ts).GetAdapter();
        }

        public void DefineManifestResource(string filename, FileStream stream, ResourceAttributes attributes)
        {
            Adaptee.DefineManifestResource(filename, stream, attributes);
        }
    }
#endif
}