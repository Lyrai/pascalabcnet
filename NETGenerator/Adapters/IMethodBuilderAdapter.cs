using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    #if NETCOREAPP
    public class UnmanagedMarshal
    {
        public static UnmanagedMarshal DefineUnmanagedMarshal(object obj)
        {
            return new UnmanagedMarshal();
        }
    }
    #endif
    public interface IMethodBuilderAdapter: IMethodInfoAdapter
    {
        IILGeneratorAdapter GetILGenerator();
        void SetCustomAttribute(IConstructorInfoAdapter con, byte[] binaryAttribute);
        void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute);
        IParameterBuilderAdapter DefineParameter(int index, ParameterAttributes attributes, string name);
        void DefineGenericParameters(string[] names);
        void SetReturnType(ITypeAdapter type);
        void SetParameters(ITypeAdapter[] types);
        void SetMarshal(UnmanagedMarshal marshal);
        void SetImplementationFlags(MethodImplAttributes attributes);
        ITypeAdapter[] GetGenericArguments();
    }
}