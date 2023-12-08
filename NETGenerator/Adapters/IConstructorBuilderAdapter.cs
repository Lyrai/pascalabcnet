using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IConstructorBuilderAdapter: IConstructorInfoAdapter
    {
        TypeAdapter DeclaringType { get; }
        
        IILGeneratorAdapter GetILGenerator();
        IParameterBuilderAdapter DefineParameter(int position, ParameterAttributes attributes, string name);
        void SetImplementationFlags(MethodImplAttributes attributes);
    }
}