using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynConstructorBuilderAdapter: RoslynConstructorInfoAdapter, IConstructorBuilderAdapter
    {
        private RoslynILGeneratorAdapter _ilGenerator;
        public RoslynConstructorBuilderAdapter(MethodAttributes attributes, ITypeAdapter declaringType, ITypeAdapter[] parameters) : base(attributes, declaringType, parameters)
        { }

        public IILGeneratorAdapter GetILGenerator()
        {
            if (_ilGenerator is null)
            {
                _ilGenerator = new RoslynILGeneratorAdapter();
            }

            return _ilGenerator;
        }

        public IParameterBuilderAdapter DefineParameter(int position, ParameterAttributes attributes, string name)
        {
            var parameter = new RoslynParameterBuilderAdapter(name, attributes, position);
            _parameterBuilders.Insert(0, parameter);

            return parameter;
        }

        public void SetImplementationFlags(MethodImplAttributes attributes)
        {
            throw new System.NotImplementedException();
        }
    }
}