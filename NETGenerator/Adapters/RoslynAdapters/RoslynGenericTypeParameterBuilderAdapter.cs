using System;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynGenericTypeParameterBuilderAdapter: RoslynTypeAdapter, IGenericTypeParameterBuilderAdapter
    {
        public TypeSymbol Symbol { get; private set; }
        public override bool IsGenericType => !IsArray;
        public override bool IsGenericParameter => !IsArray;

        private RoslynGenericTypeParameterBuilderAdapter _byRefType;

        public RoslynGenericTypeParameterBuilderAdapter(ITypeAdapter declaringType, string name): base(declaringType.Module, name, TypeAttributes.Class, null, null)
        {
        }
        
        private RoslynGenericTypeParameterBuilderAdapter(IModuleAdapter module, string name, TypeAttributes attr, ITypeAdapter parent, ITypeAdapter[] interfaces) : base(module, name, attr, parent, interfaces)
        {
        }

        protected RoslynGenericTypeParameterBuilderAdapter(ITypeBuilderAdapter declaringType, string name, TypeAttributes attr) : base(declaringType, name, attr)
        {
        }
        
        public override ITypeAdapter MakeByRefType()
        {
            if (_byRefType is null)
            {
                _byRefType = new RoslynGenericTypeParameterBuilderAdapter(Module, Name + "&", Attributes, BaseType, _interfaces.ToArray());
                if (!(Symbol is null))
                {
                    _byRefType.SetSymbol(Symbol);
                }
            }

            return _byRefType;
        }

        public void SetInterfaceConstraints(ITypeAdapter[] interfaces)
        {
            Console.WriteLine("RoslynGenericTypeParameterBuilderAdapter.SetInterfaceConstraints not implemented");
            //throw new System.NotImplementedException();
        }

        public void SetGenericParameterAttributes(GenericParameterAttributes attributes)
        {
            Console.WriteLine("RoslynGenericTypeParameterBuilderAdapter.SetGenericParameterAttributes not implemented");
            //throw new System.NotImplementedException();
        }

        public void SetBaseTypeConstraint(ITypeAdapter baseType)
        {
            Console.WriteLine("RoslynGenericTypeParameterBuilderAdapter.SetBaseTypeConstraint not implemented");
            //throw new System.NotImplementedException();
        }

        public void SetSymbol(TypeSymbol symbol)
        {
            Symbol = symbol;
            if (!(_byRefType is null))
            {
                _byRefType.SetSymbol(Symbol);
            }
        }
    }
}