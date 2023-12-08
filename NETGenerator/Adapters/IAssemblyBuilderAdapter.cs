using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IAssemblyBuilderAdapter
    {
        void DefineVersionInfoResource(string product, string productVersion, string company, string copyright, string trademark);
        void DefineUnmanagedResource(string path);
        void DefineUnmanagedResource(byte[] path);
        IModuleBuilderAdapter DefineDynamicModule(string name, bool emitSymbolInfo);
        IModuleBuilderAdapter DefineDynamicModule(string name, string fileName, bool emitSymbolInfo);
        void SetCustomAttribute(IConstructorInfoAdapter con, byte[] binaryAttribute);
        void SetEntryPoint(IMethodBuilderAdapter method, PEFileKinds fileKind);
        void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute);
        TypeAdapter CreateInstance(string typeName);
        void Save(string filename);
        void Save(string filename, PortableExecutableKinds peKind, ImageFileMachine imageFile);
    }
}