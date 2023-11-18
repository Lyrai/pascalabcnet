using System.Reflection;

namespace NETGenerator.Adapters
{
    public interface IAssemblyBuilderAdapter
    {
        void DefineVersionInfoResource(string product, string productVersion, string company, string copyright, string trademark);
        void DefineUnmanagedResource(string path);
        void DefineUnmanagedResource(byte[] path);
        IModuleBuilderAdapter DefineDynamicModule(string name, bool emitSymbolInfo);
        IModuleBuilderAdapter DefineDynamicModule(string name, string fileName, bool emitSymbolInfo);
        void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute);
    }
}