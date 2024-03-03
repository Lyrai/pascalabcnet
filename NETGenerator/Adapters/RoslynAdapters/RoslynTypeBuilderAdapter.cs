using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Roslyn.Utilities;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynTypeBuilderAdapter: RoslynTypeAdapter, ITypeBuilderAdapter
    {
        public SingleTypeDeclaration Declaration { get; private set; }
        public RoslynTypeBuilderAdapter(IModuleAdapter module, string name, TypeAttributes attr, ITypeAdapter parent, ITypeAdapter[] interfaces) 
            : base(module, name, attr, parent, interfaces)
        { }

        private RoslynTypeBuilderAdapter(RoslynTypeBuilderAdapter declaringType, string name, TypeAttributes attr)
            : base(declaringType, name, attr)
        { }

        public IConstructorBuilderAdapter DefineConstructor(MethodAttributes attributes, CallingConventions conventions,
            ITypeAdapter[] parameterTypes)
        {
            if (Declaration is object)
            {
                throw new InvalidOperationException("Type has already been created");
            }
            
            if (parameterTypes is null)
            {
                parameterTypes = new ITypeAdapter[] { };
            }

            if (FindConstructorBySignature(parameterTypes) is object)
            {
                throw new InvalidOperationException("Constructor exists");
            }

            var constructor = new RoslynConstructorBuilderAdapter(attributes, this, parameterTypes);
            if(_members.TryGetValue(".ctor", out var members))
            {
                members.Add(constructor);
            }
            else
            {
                _members.Add(".ctor", new List<IMemberInfoAdapter> { constructor });
            }

            return constructor;
        }

        public IFieldBuilderAdapter DefineField(string name, ITypeAdapter type, FieldAttributes attributes)
        {
            if (Declaration is object)
            {
                throw new InvalidOperationException("Type has already been created");
            }

            if (_members.TryGetValue(name, out var members) && members.Any(member => member is IFieldInfoAdapter))
            {
                throw new InvalidOperationException("Field exists");
            }

            var field = new RoslynFieldBuilderAdapter(name, type, attributes);
            if (members is object)
            {
                members.Add(field);
            }
            else
            {
                _members.Add(name, new List<IMemberInfoAdapter> { field });
            }
            
            return field;
        }

        public IConstructorBuilderAdapter DefineDefaultConstructor(MethodAttributes attributes)
        {
            return DefineConstructor(attributes, CallingConventions.Any, null);
        }

        public IMethodBuilderAdapter DefineMethod(string name, MethodAttributes attributes, ITypeAdapter returnType,
            ITypeAdapter[] parameterTypes)
        {
            if (Declaration is object)
            {
                throw new InvalidOperationException("Type has already been created");
            }
            
            if (FindMethodBySignature(name, parameterTypes) is object)
            {
                throw new InvalidOperationException("Method exists");
            }
            var method = new RoslynMethodBuilderAdapter(name, attributes, returnType, parameterTypes);
            if(_members.TryGetValue(name, out var members))
            {
                members.Add(method);
            }
            else
            {
                _members.Add(name, new List<IMemberInfoAdapter> { method });
            }
            
            return method;
        }

        public IMethodBuilderAdapter DefineMethod(string name, MethodAttributes attributes)
        {
            return DefineMethod(name, attributes, null, null);
        }

        public IGenericTypeParameterBuilderAdapter[] DefineGenericParameters(string[] names)
        {
            throw new System.NotImplementedException();
        }

        public IEventBuilderAdapter DefineEvent(string name, EventAttributes attributes, ITypeAdapter type)
        {
            throw new System.NotImplementedException();
        }

        public IPropertyBuilderAdapter DefineProperty(string name, PropertyAttributes attributes, ITypeAdapter returnType,
            ITypeAdapter[] parameterTypes)
        {
            if (Declaration is object)
            {
                throw new InvalidOperationException("Type has already been created");
            }
            
            if (_members.TryGetValue(name, out var members) && members.Any(member => member is IPropertyInfoAdapter))
            {
                throw new InvalidOperationException("Property exists");
            }

            var property = new RoslynPropertyBuilderAdapter(name, attributes);
            if (members is object)
            {
                members.Add(property);
            }
            else
            {
                _members.Add(name, new List<IMemberInfoAdapter> { property });
            }

            return property;
        }

        public IMethodBuilderAdapter DefinePInvokeMethod(string name, string dllName, string entryName, MethodAttributes attributes,
            CallingConventions callingConvention, ITypeAdapter returnType, ITypeAdapter[] parameterTypes,
            CallingConvention nativeCallConv, CharSet nativeCharSet)
        {
            throw new System.NotImplementedException();
        }

        public ITypeBuilderAdapter DefineNestedType(string name, TypeAttributes attributes)
        {
            if (Declaration is object)
            {
                throw new InvalidOperationException("Type has already been created");
            }
            
            if (_netstedTypes.ContainsKey(name))
            {
                throw new InvalidOperationException("Type exists");
            }

            var type = new RoslynTypeBuilderAdapter(this, name, attributes);
            _netstedTypes.Add(name, type);

            return type;
        }

        public void DefineMethodOverride(IMethodInfoAdapter methodBody, IMethodInfoAdapter methodInfo)
        {
            throw new System.NotImplementedException();
        }

        public IConstructorBuilderAdapter DefineTypeInitializer()
        {
            throw new System.NotImplementedException();
        }

        public void AddInterfaceImplementation(ITypeAdapter type)
        {
            throw new System.NotImplementedException();
        }

        public void SetCustomAttribute(IConstructorInfoAdapter constructor, byte[] attribute)
        {
            throw new System.NotImplementedException();
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter constructor)
        {
            throw new System.NotImplementedException();
        }

        public void SetParent(ITypeAdapter type)
        {
            BaseType = type;
        }

        public ITypeAdapter CreateType()
        {
            if (Declaration is null)
            {
                CreateDeclaration();
            }

            return this;
        }

        private SingleTypeDeclaration CreateDeclaration()
        {
            var memberNames = _members
                .Keys
                .Select(name => (name, new VoidResult()).ToKeyValuePair())
                .ToImmutableSegmentedDictionary();

            var declarationModifiers = DeclarationModifiers.None;
            if (IsAbstract) declarationModifiers |= DeclarationModifiers.Abstract;
            if (IsPublic) declarationModifiers |= DeclarationModifiers.Public;
            if (IsSealed) declarationModifiers |= DeclarationModifiers.Sealed;

            var typeDeclarationFlags = SingleTypeDeclaration.TypeDeclarationFlags.None;
            if (_members.Any()) typeDeclarationFlags |= SingleTypeDeclaration.TypeDeclarationFlags.HasAnyNontypeMembers;

            DeclarationKind declarationKind = DeclarationKind.Struct;
            if (IsClass) declarationKind = DeclarationKind.Class;
            else if (IsInterface) declarationKind = DeclarationKind.Interface;
            
            var nestedTypes = _netstedTypes
                .Values
                .Select(type => (type as RoslynTypeBuilderAdapter).CreateDeclaration())
                .ToImmutableArray();
            
            Declaration = new SingleTypeDeclaration(
                declarationKind,
                Name,
                0,
                declarationModifiers,
                typeDeclarationFlags,
                null,
                new RoslynSourceLocation(0, 0),
                memberNames,
                nestedTypes,
                ImmutableArray<Diagnostic>.Empty,
                QuickAttributes.None
            );

            return Declaration;
        }
    }
}