using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
        public override bool IsGenericType => _isGenericType;
        public override bool IsGenericTypeDefinition => _isGenericTypeDefinition;

        private ITypeAdapter[] _genericParameters;
        private ITypeAdapter[] _genericArguments;
        private bool _isGenericType;
        private bool _isGenericTypeDefinition;
        private ITypeAdapter _genericDefinition;
        private Dictionary<ITypeAdapter, ITypeAdapter> _instantiationDict;

        public RoslynTypeBuilderAdapter(IModuleAdapter module, string name, TypeAttributes attr, ITypeAdapter parent,
            ITypeAdapter[] interfaces)
            : base(module, name, attr, parent, interfaces)
        {
            _genericParameters = Array.Empty<ITypeAdapter>();
            _genericArguments = Array.Empty<ITypeAdapter>();
            _isGenericType = false;
            _isGenericTypeDefinition = false;
            _genericDefinition = null;
        }

        private RoslynTypeBuilderAdapter(RoslynTypeBuilderAdapter declaringType, string name, TypeAttributes attr)
            : base(declaringType, name, attr)
        {
            _genericParameters = Array.Empty<ITypeAdapter>();
            _genericArguments = Array.Empty<ITypeAdapter>();
            _isGenericType = false;
            _isGenericTypeDefinition = false;
            _genericDefinition = null;
        }

        private RoslynTypeBuilderAdapter(RoslynTypeBuilderAdapter genericDefinition, ITypeAdapter[] typeArguments)
            : base(genericDefinition.Module, genericDefinition.FullName, genericDefinition.Attributes, genericDefinition.BaseType, genericDefinition._interfaces.ToArray())
        {
            _genericArguments = typeArguments;
            _genericParameters = Array.Empty<ITypeAdapter>();
            _isGenericType = true;
            _isGenericTypeDefinition = false;
            _genericDefinition = genericDefinition;
            _members = genericDefinition._members;
        }

        public override ITypeAdapter MakeGenericType(params ITypeAdapter[] types)
        {
            return new RoslynTypeBuilderAdapter(this, types);
        }

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

            if (FindConstructorBySignature(parameterTypes) is IConstructorBuilderAdapter cb)
            {
                return cb;
            }

            var constructor = new RoslynConstructorBuilderAdapter(attributes, this, parameterTypes);
            if(_members.TryGetValue(constructor.Name, out var members))
            {
                members.Add(constructor);
            }
            else
            {
                _members.Add(constructor.Name, new List<IMemberInfoAdapter> { constructor });
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
                return members.First(member => member is IFieldInfoAdapter) as IFieldBuilderAdapter;
            }

            var field = new RoslynFieldBuilderAdapter(this, name, type, attributes);
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

        public override ITypeAdapter MakeByRefType()
        {
            return new RoslynTypeBuilderAdapter(Module, FullName + "&", Attributes, BaseType, _interfaces.ToArray());
        }

        public IConstructorBuilderAdapter DefineDefaultConstructor(MethodAttributes attributes)
        {
            return DefineConstructor(attributes, CallingConventions.Standard, null);
        }

        public IMethodBuilderAdapter DefineMethod(string name, MethodAttributes attributes, ITypeAdapter returnType,
            ITypeAdapter[] parameterTypes)
        {
            if (Declaration is object)
            {
                throw new InvalidOperationException("Type has already been created");
            }
            
            var method = new RoslynMethodBuilderAdapter(this, name, attributes, returnType, parameterTypes);
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
            var genericParameters = names
                .Select(name => new RoslynGenericTypeParameterBuilderAdapter(this, name))
                .Cast<IGenericTypeParameterBuilderAdapter>()
                .ToArray();

            _genericParameters = genericParameters
                .Cast<ITypeAdapter>()
                .ToArray();

            _isGenericType = true;
            _isGenericTypeDefinition = true;
            
            return genericParameters;
        }

        public override ITypeAdapter[] GetGenericArguments()
        {
            return _genericArguments.Length > 0 ? _genericArguments : _genericParameters;
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
            Debug.Assert(methodBody is RoslynMethodBuilderAdapter);
            ((RoslynMethodBuilderAdapter) methodBody).SetOverride(methodInfo);
        }

        public IConstructorBuilderAdapter DefineTypeInitializer()
        {
            MethodAttributes attributes = MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.SpecialName;
            return DefineConstructor(attributes, CallingConventions.Standard, null);
        }

        public void AddInterfaceImplementation(ITypeAdapter type)
        {
            _interfaces.Add(type);
        }

        public void SetCustomAttribute(IConstructorInfoAdapter constructor, byte[] attribute)
        {
            Console.WriteLine("RoslynTypeBuilderAdapter.SetCustomAttribute not implemented");
            //throw new System.NotImplementedException();
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter constructor)
        {
            throw new System.NotImplementedException();
        }

        public void SetParent(ITypeAdapter type)
        {
            BaseType = type;
        }

        public override IFieldInfoAdapter GetField(string name, BindingFlags flags)
        {
            if (IsGenericType && !IsGenericTypeDefinition && !IsGenericParameter)
            {
                var field = _genericDefinition.GetField(name, flags);
                return field.Instantiate(MakeInstantiationDict(), this) as IFieldInfoAdapter;
            }

            return base.GetField(name, flags);
        }

        public override IMethodInfoAdapter GetMethod(string name)
        {
            if (IsGenericType && !IsGenericTypeDefinition && !IsGenericParameter)
            {
                var method = _genericDefinition.GetMethod(name);
                return method.Instantiate(MakeInstantiationDict(), this) as IMethodInfoAdapter;
            }

            return base.GetMethod(name);
        }

        public override IMethodInfoAdapter GetMethod(string name, ITypeAdapter[] parameterTypes)
        {
            if (IsGenericType && !IsGenericTypeDefinition && !IsGenericParameter)
            {
                var method = _genericDefinition.GetMethod(name, parameterTypes);
                return method.Instantiate(MakeInstantiationDict(), this) as IMethodInfoAdapter;
            }

            return base.GetMethod(name, parameterTypes);
        }

        public override IMethodInfoAdapter GetMethod(string name, BindingFlags flags)
        {
            if (IsGenericType && !IsGenericTypeDefinition && !IsGenericParameter)
            {
                var method = _genericDefinition.GetMethod(name, flags);
                return method.Instantiate(MakeInstantiationDict(), this) as IMethodInfoAdapter;
            }

            return base.GetMethod(name, flags);
        }

        public override IConstructorInfoAdapter GetConstructor(ITypeAdapter[] parameterTypes)
        {
            if (IsGenericType && !IsGenericTypeDefinition && !IsGenericParameter)
            {
                var constructor = _genericDefinition.GetConstructor(parameterTypes);
                return constructor.Instantiate(MakeInstantiationDict(), this) as IConstructorInfoAdapter;
            }

            return base.GetConstructor(parameterTypes);
        }

        public override IMethodInfoAdapter[] GetMethods()
        {
            if (IsGenericType && !IsGenericTypeDefinition && !IsGenericParameter)
            {
                var methods = _genericDefinition.GetMethods();
                return methods
                    .Select(method => method.Instantiate(MakeInstantiationDict(), this))
                    .Cast<IMethodInfoAdapter>()
                    .ToArray();
            }
            
            return base.GetMethods();
        }
        
        public override IPropertyInfoAdapter[] GetProperties()
        {
            if (IsGenericType && !IsGenericTypeDefinition && !IsGenericParameter)
            {
                var properties = _genericDefinition.GetProperties();
                return properties
                    .Select(property => property.Instantiate(MakeInstantiationDict(), this))
                    .Cast<IPropertyInfoAdapter>()
                    .ToArray();
            }
            
            return base.GetProperties();
        }

        public override IMethodInfoAdapter[] GetMethods(BindingFlags flags)
        {
            if (IsGenericType && !IsGenericTypeDefinition && !IsGenericParameter)
            {
                var methods = _genericDefinition.GetMethods(flags);
                return methods
                    .Select(method => method.Instantiate(MakeInstantiationDict(), this))
                    .Cast<IMethodInfoAdapter>()
                    .ToArray();
            }
            
            return base.GetMethods(flags);
        }

        public override IConstructorInfoAdapter[] GetConstructors()
        {
            if (IsGenericType && !IsGenericTypeDefinition && !IsGenericParameter)
            {
                var constructors = _genericDefinition.GetConstructors();
                return constructors
                    .Select(method => method.Instantiate(MakeInstantiationDict(), this))
                    .Cast<IConstructorInfoAdapter>()
                    .ToArray();
            }
            
            return base.GetConstructors();
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
            if (IsInterface) declarationKind = DeclarationKind.Interface;
            
            var nestedTypes = _netstedTypes
                .Values
                .Select(type => (type as RoslynTypeBuilderAdapter).CreateDeclaration())
                .ToImmutableArray();

            Declaration = new SingleTypeDeclaration(
                declarationKind,
                Name,
                _genericParameters.Length + _genericArguments.Length,
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

        private Dictionary<ITypeAdapter, ITypeAdapter> MakeInstantiationDict()
        {
            if (_instantiationDict is null)
            {
                _instantiationDict = new Dictionary<ITypeAdapter, ITypeAdapter>();
                var parameters = _genericDefinition.GetGenericArguments();
                Debug.Assert(parameters.Length == _genericArguments.Length);
                for (int i = 0; i < parameters.Length; ++i)
                {
                    _instantiationDict.Add(parameters[i], _genericArguments[i]);
                }
            }

            return _instantiationDict;
        }
    }
}