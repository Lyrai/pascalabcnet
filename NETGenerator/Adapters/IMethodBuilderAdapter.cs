using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IMethodBuilderAdapter: IMethodInfoAdapter
    {
        IILGeneratorAdapter GetILGenerator();
        void SetCustomAttribute(IConstructorInfoAdapter con, byte[] binaryAttribute);
        void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute);
        IParameterBuilderAdapter DefineParameter(int index, ParameterAttributes attributes, string name);
        void DefineGenericParameters(string[] names);
        void SetReturnType(TypeAdapter type);
        void SetParameters(TypeAdapter[] types);
        void SetMarshal(UnmanagedMarshal marshal);
        void SetImplementationFlags(MethodImplAttributes attributes);
        TypeAdapter[] GetGenericArguments();
    }
}