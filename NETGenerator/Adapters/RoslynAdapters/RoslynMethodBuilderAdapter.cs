using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynMethodBuilderAdapter: RoslynMethodInfoAdapter, IMethodBuilderAdapter
    {
        private RoslynILGeneratorAdapter _ilGenerator;
        public RoslynMethodBuilderAdapter(string name, MethodAttributes attributes, ITypeAdapter returnType, ITypeAdapter[] parameterTypes) : base(name, attributes, returnType, parameterTypes)
        { }
        
        public IILGeneratorAdapter GetILGenerator()
        {
            if (_ilGenerator is null)
            {
                _ilGenerator = new RoslynILGeneratorAdapter();
            }

            return _ilGenerator;
        }

        public void SetCustomAttribute(IConstructorInfoAdapter con, byte[] binaryAttribute)
        {
            throw new System.NotImplementedException();
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            throw new System.NotImplementedException();
        }

        public IParameterBuilderAdapter DefineParameter(int index, ParameterAttributes attributes, string name)
        {
            var parameter = new RoslynParameterBuilderAdapter(name, attributes, index);
            _parameterBuilders.Insert(0, parameter);

            return parameter;
        }

        public void DefineGenericParameters(string[] names)
        {
            throw new System.NotImplementedException();
        }

        public void SetReturnType(ITypeAdapter type)
        {
            ReturnType = type;
        }

        public void SetParameters(ITypeAdapter[] types)
        {
            _parameters.Clear();
            foreach (var type in types)
            {
                _parameters.Add(new RoslynParameterInfoAdapter(type));
            }
        }

        public void SetMarshal(UnmanagedMarshal marshal)
        {
            throw new System.NotImplementedException();
        }

        public void SetImplementationFlags(MethodImplAttributes attributes)
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter[] GetGenericArguments()
        {
            throw new System.NotImplementedException();
        }
    }
}