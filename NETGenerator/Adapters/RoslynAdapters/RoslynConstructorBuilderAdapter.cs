using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynConstructorBuilderAdapter: RoslynConstructorInfoAdapter, IConstructorBuilderAdapter
    {
        private RoslynILGeneratorAdapter _ilGenerator;

        public RoslynConstructorBuilderAdapter(MethodAttributes attributes, ITypeAdapter declaringType,
            ITypeAdapter[] parameters) : base(attributes, declaringType, parameters)
        {
            _parameterBuilders = new List<IParameterBuilderAdapter>
            {
                new RoslynParameterBuilderAdapter("", ParameterAttributes.None, 0, DeclaringType)
            };
            
            if(parameters is object)
            {
                for (int i = 0; i < parameters.Length; ++i)
                {
                    _parameterBuilders.Add(new RoslynParameterBuilderAdapter("", ParameterAttributes.None, i + 1,
                        parameters[i]));
                }
            }
        }

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
            var old = _parameterBuilders[position] as RoslynParameterBuilderAdapter;
            _parameterBuilders[position] = new RoslynParameterBuilderAdapter(name, attributes, position, old.ParameterType);
            
            return _parameterBuilders[position];
        }

        public override IParameterInfoAdapter[] GetParameters()
        {
            var result = new IParameterInfoAdapter[_parameterBuilders.Count - 1];
            for (int i = 1; i < _parameterBuilders.Count; ++i)
            {
                result[i - 1] = (_parameterBuilders[i] as RoslynParameterBuilderAdapter).GetInfo();
            }

            return result;
        }

        public void SetImplementationFlags(MethodImplAttributes attributes)
        {
            Console.WriteLine("RoslynConstructorBuilderAdapter.SetImplementationFlags not implemented");
            //throw new System.NotImplementedException();
        }

        public override IMemberInfoAdapter Instantiate(Dictionary<ITypeAdapter, ITypeAdapter> typeArguments, ITypeAdapter declaringType)
        {
            var instantiatedParameters = _parameterBuilders
                .Cast<RoslynParameterBuilderAdapter>()
                .Select(param => typeArguments.TryGetValue(param.ParameterType, out var instance) ? param.WithTypeSubstituted(instance) : param)
                .ToList();

            var result = new RoslynConstructorBuilderAdapter(Attributes, declaringType, null);
            result._parameterBuilders = instantiatedParameters;
            
            // TODO Probably need to instantiate IL builder

            return result;
        }
    }
}