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

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynGenericTypeAdapter: ITypeAdapter
    {
        public bool IsGenericType { get; }
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
        public bool IsByRef => Adaptee.IsReferenceType;
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
        public NamedTypeSymbol Adaptee { get; }
        public IEnumerable<ITypeAdapter> ImplementedInterfaces => Enumerable.Empty<ITypeAdapter>();

        public RoslynGenericTypeAdapter(AssemblySymbol assembly, Type type)
        {
            Adaptee = assembly.GetTypeByMetadataName(type.Namespace + "." + type.Name, true, false, out _);
            if (type.GenericTypeArguments.Length > 0)
            {
                var arguments = type.GenericTypeArguments
                        .Select(param => assembly.GetTypeByMetadataName(param.Namespace + "." + param.Name, true, false, out _) as TypeSymbol)
                        .Select(param => TypeWithAnnotations.Create(param))
                        .ToImmutableArray();
                
                Adaptee = Adaptee.ConstructIfGeneric(arguments);
            } 
            else
            {
                Adaptee = Adaptee.ConstructUnboundGenericType();
            }
            
            Debug.Assert(Adaptee is object);
            ConstructedFrom = type;
            Attributes = type.Attributes;
            BaseType = type.BaseType.GetAdapter();
        }

        private RoslynGenericTypeAdapter(RoslynGenericTypeAdapter source, ITypeAdapter[] genericTypes)
        {
            ConstructedFrom = source.ConstructedFrom;
            Adaptee = source.Adaptee;
        }
        
        public IMethodInfoAdapter GetMethod(string name)
        {
            throw new NotImplementedException();
        }

        public IMethodInfoAdapter GetMethod(string name, ITypeAdapter[] parameterTypes)
        {
            throw new System.NotImplementedException();
        }

        public IMethodInfoAdapter GetMethod(string name, BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public IMethodInfoAdapter[] GetMethods()
        {
            throw new System.NotImplementedException();
        }

        public IMethodInfoAdapter[] GetMethods(BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public IConstructorInfoAdapter GetConstructor(ITypeAdapter[] parameterTypes)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public ITypeAdapter GetElementType()
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter GetGenericTypeDefinition()
        {
            throw new System.NotImplementedException();
        }

        public IPropertyInfoAdapter GetProperty(string name)
        {
            throw new System.NotImplementedException();
        }

        public IPropertyInfoAdapter GetProperty(string name, BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public IFieldInfoAdapter GetField(string name, BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter GetInterface(string name)
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter[] GetInterfaces()
        {
            throw new System.NotImplementedException();
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
            return ConstructedFrom.MakeArrayType().GetAdapter();
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