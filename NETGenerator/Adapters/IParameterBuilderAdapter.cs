using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IParameterBuilderAdapter: IAdapter
    {
        int Position { get; }
        string Name { get; }
        
        void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute);
        void SetCustomAttribute(IConstructorInfoAdapter constructor, byte[] binaryAttribute);
        void SetConstant(object value);
    }
}