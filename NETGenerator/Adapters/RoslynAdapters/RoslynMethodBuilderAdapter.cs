using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Emit;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynMethodBuilderAdapter: RoslynMethodInfoAdapter, IMethodBuilderAdapter
    {
        public IMethodInfoAdapter Overrides { get; private set; }

        private RoslynILGeneratorAdapter _ilGenerator;
        private ITypeAdapter[] _genericParameters;
        private ITypeAdapter[] _genericArguments;
        private List<IParameterBuilderAdapter> _parameterBuilders;

        public RoslynMethodBuilderAdapter(ITypeAdapter declaringType, string name, MethodAttributes attributes,
            ITypeAdapter returnType, ITypeAdapter[] parameterTypes) : base(name, attributes, returnType, parameterTypes)
        {
            DeclaringType = declaringType;
            _parameterBuilders = new List<IParameterBuilderAdapter>
            {
                new RoslynParameterBuilderAdapter("", ParameterAttributes.None, 0, returnType)
            };
            if(parameterTypes is object)
            {
                for (int i = 0; i < parameterTypes.Length; ++i)
                {
                    _parameterBuilders.Add(new RoslynParameterBuilderAdapter("", ParameterAttributes.None, i + 1,
                        parameterTypes[i]));
                }
            }

            _genericArguments = Array.Empty<ITypeAdapter>();
            _genericParameters = Array.Empty<ITypeAdapter>();
        }

        private RoslynMethodBuilderAdapter(RoslynMethodBuilderAdapter definition, ITypeAdapter[] genericArguments)
            : base(definition.Name, definition.Attributes, definition.ReturnType, Array.Empty<ITypeAdapter>())
        {
            DeclaringType = definition.DeclaringType;
            _parameterBuilders = definition._parameterBuilders;
            _genericArguments = genericArguments;
            _genericParameters = Array.Empty<ITypeAdapter>();
            IsGenericMethod = true;
            IsGenericMethodDefinition = false;
        }
        
        public IILGeneratorAdapter GetILGenerator()
        {
            if (_ilGenerator is null)
            {
                _ilGenerator = new RoslynILGeneratorAdapter();
            }

            return _ilGenerator;
        }

        public void SetOverride(IMethodInfoAdapter method)
        {
            Overrides = method;
        }

        public override IMemberInfoAdapter Instantiate(Dictionary<ITypeAdapter, ITypeAdapter> typeArguments, ITypeAdapter declaringType)
        {
            var instantiatedParameters = _parameterBuilders
                .Cast<RoslynParameterBuilderAdapter>()
                .Select(param => typeArguments.TryGetValue(param.ParameterType, out var instance) ? param.WithTypeSubstituted(instance) : param)
                .ToList();

            var result = new RoslynMethodBuilderAdapter(declaringType, Name, Attributes, ReturnType, null);
            result._parameterBuilders = instantiatedParameters;

            // TODO Probably need to instantiate IL builder

            return result;
        }

        public void SetCustomAttribute(IConstructorInfoAdapter con, byte[] binaryAttribute)
        {
            Console.WriteLine("RoslynMethodBuilderAdapter.SetCustomAttribute not implemented");
            //throw new System.NotImplementedException();
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            Console.WriteLine("RoslynMethodBuilderAdapter.SetCustomAttribute not implemented");
            //throw new System.NotImplementedException();
        }

        public IParameterBuilderAdapter DefineParameter(int index, ParameterAttributes attributes, string name)
        {
            var old = _parameterBuilders[index] as RoslynParameterBuilderAdapter;
            _parameterBuilders[index] = new RoslynParameterBuilderAdapter(name, attributes, index, old.ParameterType);
            
            return _parameterBuilders[index];
        }

        public void DefineGenericParameters(string[] names)
        {
            _genericParameters = names
                .Select(name => new RoslynGenericTypeParameterBuilderAdapter(DeclaringType, name))
                .Cast<ITypeAdapter>()
                .ToArray();

            IsGenericMethod = true;
            IsGenericMethodDefinition = true;
        }

        public void SetReturnType(ITypeAdapter type)
        {
            ReturnType = type;
            var old = _parameterBuilders[0] as RoslynParameterBuilderAdapter;
            _parameterBuilders[0] =
                new RoslynParameterBuilderAdapter(old.Name, old.Attributes, old.Position, old.ParameterType);
        }

        public void SetParameters(ITypeAdapter[] types)
        {
            _parameterBuilders.Clear();
            _parameterBuilders.Add(new RoslynParameterBuilderAdapter("", ParameterAttributes.None, 0, ReturnType));
            for (int i = 0; i < types.Length; ++i)
            {
                _parameterBuilders.Add(new RoslynParameterBuilderAdapter("", ParameterAttributes.None, i + 1, types[i]));
            }
        }

        public void SetMarshal(UnmanagedMarshal marshal)
        {
            throw new System.NotImplementedException();
        }

        public void SetImplementationFlags(MethodImplAttributes attributes)
        {
            Console.WriteLine("RoslynMethodBuilderAdapter.SetImplementationFlags not implemented");
            //throw new System.NotImplementedException();
        }

        public override ITypeAdapter[] GetGenericArguments()
        {
            return _genericParameters.Length > 0 ? _genericParameters : _genericArguments;
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

        public override IMethodInfoAdapter MakeGenericMethod(params ITypeAdapter[] types)
        {
            return new RoslynMethodBuilderAdapter(this, types);
        }
    }
}