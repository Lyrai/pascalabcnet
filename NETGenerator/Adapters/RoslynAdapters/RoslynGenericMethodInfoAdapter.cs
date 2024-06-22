using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using NETGenerator.Adapters.Utility;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynGenericMethodInfoAdapter: IMethodInfoAdapter
    {
        public string Name => ConstructedFrom.Name;
        public int MetadataToken => ConstructedFrom.MetadataToken;
        public bool IsSpecialName => ConstructedFrom.IsSpecialName;
        public bool IsPublic => ConstructedFrom.IsPublic;
        public bool IsPrivate => ConstructedFrom.IsPrivate;
        public bool IsGenericMethod => true;
        public bool IsGenericMethodDefinition { get; }
        public bool IsAssembly => ConstructedFrom.IsAssembly;
        public bool IsFamily => ConstructedFrom.IsFamily;
        public bool IsFamilyAndAssembly => ConstructedFrom.IsFamilyAndAssembly;
        public bool IsFamilyOrAssembly => ConstructedFrom.IsFamilyOrAssembly;
        public bool IsStatic => ConstructedFrom.IsStatic;
        public bool IsVirtual => ConstructedFrom.IsVirtual;
        public bool IsAbstract => ConstructedFrom.IsAbstract;
        public MethodAttributes Attributes => ConstructedFrom.Attributes;
        public ITypeAdapter DeclaringType { get; }
        public ITypeAdapter ReturnType { get; }
        public MethodInfo ConstructedFrom { get; private set; }

        public MethodSymbol Adaptee
        {
            get
            {
                if (_instantiated)
                {
                    return _adaptee;
                }

                if (_argumentTypes is null || _argumentTypes.Length == 0)
                {
                    return _adaptee;
                }
                
                var symbols = _argumentTypes.Select(ResolveHelper.ResolveType).ToArray();
                if (symbols.Any(symbol => symbol is null))
                {
                    return _adaptee;
                }
                
                var types = symbols.Select(symbol => TypeWithAnnotations.Create(symbol)).ToImmutableArray();
                _adaptee = _adaptee.ConstructedFrom.ConstructIfGeneric(types);
                _instantiated = true;
                
                return _adaptee;
            }
        }

        private bool _instantiated;
        private MethodSymbol _adaptee;
        private ITypeAdapter[] _argumentTypes;
        private IMethodInfoAdapter _constructedFromAdapter;
        private IParameterInfoAdapter[] _parameters;

        public RoslynGenericMethodInfoAdapter(MethodInfo method, ITypeAdapter declaringType)
        {
            DeclaringType = declaringType;
            ConstructedFrom = method;
            ReturnType = method.ReturnType.GetAdapter();
            _parameters = ConstructedFrom.GetParameters().Select(t => t.GetAdapter()).ToArray();
            var declaring = ResolveHelper.ResolveType(declaringType);
            _adaptee = ResolveHelper.ResolveMethodInType(declaring, this);
            
            if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
            {
                var arguments = method.GetGenericArguments()
                    .Select(param => ResolveHelper.ResolveType(param.GetAdapter()))
                    .Select(param => TypeWithAnnotations.Create(param))
                    .ToImmutableArray();

                _argumentTypes = method.GetGenericArguments().Select(t => t.GetAdapter()).ToArray();
                _adaptee = _adaptee.ConstructIfGeneric(arguments);
                IsGenericMethodDefinition = false;
                _instantiated = true;
            }
            else
            {
                IsGenericMethodDefinition = true;
                _instantiated = false;
            }

            _constructedFromAdapter = this;
        }

        private RoslynGenericMethodInfoAdapter(RoslynGenericMethodInfoAdapter source, ITypeAdapter[] genericTypes)
        {
            ConstructedFrom = source.ConstructedFrom;
            DeclaringType = source.DeclaringType;
            _adaptee = source._adaptee;
            _argumentTypes = genericTypes;
            _constructedFromAdapter = source;
            IsGenericMethodDefinition = false;
            _instantiated = false;
            _parameters = source._parameters;
        }
        
        public IMethodInfoAdapter MakeGenericMethod(params ITypeAdapter[] types)
        {
            return new RoslynGenericMethodInfoAdapter(this, types);
        }

        public object[] GetCustomAttributes(ITypeAdapter type, bool inherit)
        {
            Console.WriteLine("RoslynGenericMethodInfoAdapter.GetCustomAttributes not implemented");
            return Array.Empty<object>();
        }

        public IMethodInfoAdapter GetGenericMethodDefinition()
        {
            return _constructedFromAdapter;
        }

        public IParameterInfoAdapter[] GetParameters()
        {
            return _parameters;
        }

        public void Invoke(object obj, object[] parameters)
        {
            throw new System.NotSupportedException();
        }

        public ITypeAdapter[] GetGenericArguments()
        {
            if (_argumentTypes.Length > 0)
            {
                return _argumentTypes;
            }

            return ConstructedFrom.GetGenericArguments().Select(t => t.GetAdapter()).ToArray();
        }

        public IMemberInfoAdapter Instantiate(Dictionary<ITypeAdapter, ITypeAdapter> typeArguments, ITypeAdapter declaringType)
        {
            throw new System.NotSupportedException();
        }
    }
}