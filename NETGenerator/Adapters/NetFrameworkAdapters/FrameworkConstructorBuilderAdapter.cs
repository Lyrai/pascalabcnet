using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkConstructorBuilderAdapter: FrameworkConstructorInfoAdapter, IConstructorBuilderAdapter
    {
        public ConstructorBuilder Adaptee { get; }
        
        public FrameworkConstructorBuilderAdapter(ConstructorBuilder builder) : base(builder)
        {
            Adaptee = builder;
        }

        public IILGeneratorAdapter GetILGenerator()
        {
            return Adaptee.GetILGenerator().GetAdapter();
        }

        public IParameterBuilderAdapter DefineParameter(int position, ParameterAttributes attributes, string name)
        {
            return Adaptee.DefineParameter(position, attributes, name).GetAdapter();
        }

        public void SetImplementationFlags(MethodImplAttributes attributes)
        {
            Adaptee.SetImplementationFlags(attributes);
        }
    }
}