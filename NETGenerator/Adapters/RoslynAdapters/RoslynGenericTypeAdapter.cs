using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using NETGenerator.Adapters.Utility;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynGenericTypeAdapter: ITypeAdapter
    {
        public bool IsGenericType => true;
        public bool IsArray => Adaptee.IsArray();
        public bool IsGenericTypeDefinition { get; }
        public bool IsGenericParameter { get; }
        public bool IsValueType => Adaptee.IsValueType;
        public bool IsPointer => Adaptee.IsPointerType();
        public bool IsEnum => Adaptee.IsEnumType();
        public bool IsInterface => Adaptee.IsInterfaceType();
        public bool IsClass => Adaptee.IsClassType();
        public bool IsPrimitive { get; }
        public bool IsSealed => Adaptee.IsSealed;
        public bool IsAbstract => Adaptee.IsAbstract;
        public bool IsByRef => ConstructedFrom.IsByRef;
        public bool IsNotPublic { get; }
        public IMethodInfoAdapter DeclaringMethod => null;
        public string FullName => Adaptee.ContainingNamespace.QualifiedName + "." + Adaptee.MetadataName;
        public string Name => Adaptee.Name;
        public string Namespace => Adaptee.ContainingNamespace.QualifiedName;
        public string AssemblyQualifiedName { get; }
        public AssemblyAdapter Assembly { get; }
        public int GenericParameterPosition => 0;
        public ITypeAdapter BaseType { get; }
        public ITypeAdapter DeclaringType { get; }
        public IModuleAdapter Module { get; }
        public TypeAttributes Attributes { get; }
        public Type ConstructedFrom { get; }

        public NamedTypeSymbol Adaptee
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

        public IEnumerable<ITypeAdapter> ImplementedInterfaces => Enumerable.Empty<ITypeAdapter>();

        private ITypeAdapter[] _argumentTypes;
        private bool _instantiated;
        private NamedTypeSymbol _adaptee;
        private RoslynGenericTypeAdapter _constructedFromAdapter;

        public RoslynGenericTypeAdapter(AssemblySymbol assembly, Type type)
        {
            _adaptee = assembly.GetTypeByMetadataName(type.Namespace + "." + type.Name, true, false, out _);
            if (type.GenericTypeArguments.Length > 0)
            {
                var arguments = type.GenericTypeArguments
                        .Select(param => ResolveHelper.ResolveType(param.GetAdapter()))
                        .Select(param => TypeWithAnnotations.Create(param))
                        .ToImmutableArray();

                _argumentTypes = type.GenericTypeArguments.Select(t => t.GetAdapter()).ToArray();
                _adaptee = _adaptee.ConstructIfGeneric(arguments);
                IsGenericTypeDefinition = false;
                _instantiated = true;
            } 
            else
            {
                _adaptee = _adaptee.ConstructUnboundGenericType();
                IsGenericTypeDefinition = true;
                _instantiated = false;
            }
            
            Debug.Assert(Adaptee is object);
            ConstructedFrom = type;
            Attributes = type.Attributes;
            BaseType = type.BaseType.GetAdapter();

            _constructedFromAdapter = this;
        }

        private RoslynGenericTypeAdapter(RoslynGenericTypeAdapter source, ITypeAdapter[] genericTypes)
        {
            ConstructedFrom = source.ConstructedFrom;
            _adaptee = source._adaptee;
            _argumentTypes = genericTypes;
            _constructedFromAdapter = source;
            IsGenericTypeDefinition = false;
            _instantiated = false;
        }
        
        public IMethodInfoAdapter GetMethod(string name)
        {
            var method = ConstructedFrom.GetMethod(name);
            return new RoslynNativeGenericTypeMethodInfoAdapter(method, this);
        }

        public IMethodInfoAdapter GetMethod(string name, ITypeAdapter[] parameterTypes)
        {
            var types = new List<Type>();
            foreach (var parameterType in parameterTypes)
            {
                if (parameterType is FrameworkTypeAdapter framework)
                {
                    types.Add(framework.Adaptee);
                } 
                else if (parameterType is RoslynGenericTypeAdapter generic)
                {
                    types.Add(generic.ConstructedFrom);
                }
                else
                {
                    types.Add(null);
                }
            }

            var method = ConstructedFrom.GetMethod(name, types.ToArray());
            return new RoslynNativeGenericTypeMethodInfoAdapter(method, this);
            //return method.GetAdapter();
        }

        public IMethodInfoAdapter GetMethod(string name, BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public IMethodInfoAdapter[] GetMethods()
        {
            return ConstructedFrom
                .GetMethods()
                .Select(meth => new RoslynNativeGenericTypeMethodInfoAdapter(meth, this))
                .ToArray();
            //return ConstructedFrom.GetMethods().Select(meth => meth.GetAdapter()).ToArray();
        }

        public IMethodInfoAdapter[] GetMethods(BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public IConstructorInfoAdapter GetConstructor(ITypeAdapter[] parameterTypes)
        {
            var types = new List<Type>();
            foreach (var type in parameterTypes)
            {
                if (type is FrameworkTypeAdapter framework)
                {
                    types.Add(framework.Adaptee);
                }
                else if(type is RoslynGenericTypeAdapter generic)
                {
                    types.Add(generic.ConstructedFrom);
                }
                else
                {
                    types.Add(null);
                }
            }

            var ctor = ConstructedFrom.GetConstructor(types.ToArray());
            return new RoslynNativeGenericTypeConstructorInfoAdapter(ctor, this);
            //return new RoslynNativeGenericTypeConstructorInfoAdapter(ctor, this);
            //return ctor.GetAdapter();
        }

        public IConstructorInfoAdapter[] GetConstructors()
        {
            throw new System.NotImplementedException();
        }

        public IConstructorInfoAdapter[] GetConstructors(BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter[] GetGenericArguments()
        {
            if (_argumentTypes.Length > 0)
            {
                return _argumentTypes;
            }

            return ConstructedFrom.GetGenericArguments().Select(arg => arg.GetAdapter()).ToArray();
        }

        public ITypeAdapter GetElementType()
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter GetGenericTypeDefinition()
        {
            return _constructedFromAdapter;
        }

        public IPropertyInfoAdapter GetProperty(string name)
        {
            throw new System.NotImplementedException();
        }

        public IPropertyInfoAdapter GetProperty(string name, BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public IPropertyInfoAdapter[] GetProperties()
        {
            throw new NotImplementedException();
        }

        public IFieldInfoAdapter GetField(string name, BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter GetInterface(string name)
        {
            return ConstructedFrom.GetInterface(name).GetAdapter();
        }

        public ITypeAdapter[] GetInterfaces()
        {
            return ConstructedFrom.GetInterfaces().Select(i => i.GetAdapter()).ToArray();
        }

        public ITypeAdapter[] GetNestedTypes()
        {
            throw new System.NotImplementedException();
        }

        public IMemberInfoAdapter[] GetMember(string name, BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public IFieldInfoAdapter[] GetFields()
        {
            throw new System.NotImplementedException();
        }

        public IMemberInfoAdapter[] GetDefaultMembers()
        {
            throw new System.NotImplementedException();
        }

        public IMemberInfoAdapter[] GetMembers(BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public int GetArrayRank()
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter MakeGenericType(params ITypeAdapter[] types)
        {
            return new RoslynGenericTypeAdapter(this, types);
        }

        public ITypeAdapter MakeArrayType()
        {
            return new RoslynArrayTypeAdapter(this);
        }

        public ITypeAdapter MakeArrayType(int rank)
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter MakePointerType()
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter MakeByRefType()
        {
            throw new System.NotImplementedException();
        }

        public object[] GetCustomAttributes(ITypeAdapter attributeType, bool inherit)
        {
            throw new System.NotImplementedException();
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            throw new System.NotImplementedException();
        }
    }
}