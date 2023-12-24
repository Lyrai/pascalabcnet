using System;
using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkAssemblyBuilderAdapter: FrameworkAssemblyAdapter, IAssemblyBuilderAdapter
    {
        public new AssemblyBuilder Adaptee { get; }

        public FrameworkAssemblyBuilderAdapter(AssemblyBuilder builder): base(builder)
        {
            Adaptee = builder;
        }
        
        public void DefineVersionInfoResource(string product, string productVersion, string company, string copyright,
            string trademark)
        {
            Adaptee.DefineVersionInfoResource(product, productVersion, company, copyright, trademark);
        }

        public void DefineUnmanagedResource(string path)
        {
            Adaptee.DefineUnmanagedResource(path);
        }

        public void DefineUnmanagedResource(byte[] path)
        {
            Adaptee.DefineUnmanagedResource(path);
        }

        public IModuleBuilderAdapter DefineDynamicModule(string name, bool emitSymbolInfo)
        {
            return Adaptee.DefineDynamicModule(name, emitSymbolInfo).GetAdapter();
        }

        public IModuleBuilderAdapter DefineDynamicModule(string name, string fileName, bool emitSymbolInfo)
        {
            return Adaptee.DefineDynamicModule(name, fileName, emitSymbolInfo).GetAdapter();
        }

        public void SetCustomAttribute(IConstructorInfoAdapter con, byte[] binaryAttribute)
        {
            Adaptee.SetCustomAttribute((con as FrameworkConstructorInfoAdapter).Adaptee, binaryAttribute);
        }

        public void SetEntryPoint(IMethodBuilderAdapter method, PEFileKinds fileKind)
        {
            Adaptee.SetEntryPoint((method as FrameworkMethodBuilderAdapter).Adaptee, fileKind);
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            Adaptee.SetCustomAttribute((attribute as FrameworkCustomAttributeBuilderAdapter).Adaptee);
        }

        public ITypeAdapter CreateInstance(string typeName)
        {
            return (Adaptee.CreateInstance(typeName) as Type).GetAdapter();
        }

        public void Save(string filename)
        {
            Adaptee.Save(filename);
        }

        public void Save(string filename, PortableExecutableKinds peKind, ImageFileMachine imageFile)
        {
            Adaptee.Save(filename, peKind, imageFile);
        }
    }
}