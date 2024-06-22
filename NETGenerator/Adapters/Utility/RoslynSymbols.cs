using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Cci;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslyn.Utilities;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using NETGenerator.Adapters.Utility;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;
using PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters;
using BindingDiagnosticBag = Microsoft.CodeAnalysis.CSharp.BindingDiagnosticBag;
using CallingConvention = Microsoft.Cci.CallingConvention;
using FieldSymbol = Microsoft.CodeAnalysis.CSharp.Symbols.FieldSymbol;
using NamedTypeSymbol = Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol;
using ParameterSymbol = Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol;
using Symbol = Microsoft.CodeAnalysis.CSharp.Symbol;
using TypeParameterSymbol = Microsoft.CodeAnalysis.CSharp.Symbols.TypeParameterSymbol;
using TypeSymbol = Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    class LexicalSortCounter
    {
        public static int Counter = 0;
    }

    internal abstract class PascalMethodBaseSymbol : SourceMemberMethodSymbol
    {
        public override TypeWithAnnotations ReturnTypeWithAnnotations => _returnTypeWithAnnotations;
        public override ImmutableArray<ParameterSymbol> Parameters => _parameterSymbols;
        public override string Name { get; }
        public override bool IsVararg => false;
        public override RefKind RefKind => RefKind.None;
        internal override bool IsExpressionBodied => false;
        internal override bool GenerateDebugInfo => false;

        private ImmutableArray<ParameterSymbol> _parameterSymbols;
        private LexicalSortKey _sortKey;
        protected TypeWithAnnotations _returnTypeWithAnnotations;
        
        public PascalMethodBaseSymbol(IMethodBaseAdapter method, NamedTypeSymbol containingType, bool isIterator)
            : base(containingType, null, NoLocation.Singleton, isIterator)
        {
            Name = method.Name;
            _returnTypeWithAnnotations = TypeWithAnnotations.Create(ResolveHelper.ResolveType(method.ReturnType));
            _sortKey = new LexicalSortKey(0, LexicalSortCounter.Counter++);
            
            DeclarationModifiers = DeclarationModifiers.None;
            
            if (method.IsPublic) DeclarationModifiers |= DeclarationModifiers.Public;
            else if (method.IsPrivate) DeclarationModifiers |= DeclarationModifiers.Private;
            else if (method.IsFamily) DeclarationModifiers |= DeclarationModifiers.Protected;
            else if (method.IsAssembly) DeclarationModifiers |= DeclarationModifiers.Internal;
            else if (method.IsFamilyAndAssembly) DeclarationModifiers |= DeclarationModifiers.PrivateProtected;
            else if (method.IsFamilyOrAssembly) DeclarationModifiers |= DeclarationModifiers.ProtectedInternal;
            else DeclarationModifiers |= DeclarationModifiers.Private;

            if (method.IsStatic)
            {
                DeclarationModifiers |= DeclarationModifiers.Static;
            }
        }
        
        internal override LexicalSortKey GetLexicalSortKey()
        {
            return _sortKey;
        }
        
        protected void CreateParameters(IMethodBaseAdapter method, MethodSymbol symbol)
        {
            var parameters = new List<ParameterSymbol>();
            int paramCnt = 0;
            foreach (var param in method.GetParameters().Cast<RoslynParameterInfoAdapter>())
            {
                var type = ResolveHelper.ResolveType(param.ParameterType);
                ResolveHelper.FixGenericTypeArgumentsIfNeeded(symbol, type);
                var typeWithAnnotations = TypeWithAnnotations.Create(type);
                var paramSymbol = new SourceSimpleParameterSymbol(this, typeWithAnnotations, paramCnt,
                    param.IsByRef ? RefKind.Ref : RefKind.None, ScopedKind.None, param.Name,
                    new List<Location> {NoLocation.Singleton}.ToImmutableArray());
                
                parameters.Add(paramSymbol);
                ++paramCnt;
            }

            _parameterSymbols = parameters.ToImmutableArray();
        }
        
        public override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            return ImmutableArray<ImmutableArray<TypeWithAnnotations>>.Empty;
        }

        public override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            throw new System.NotImplementedException();
        }

        protected override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
        }

        internal override ExecutableCodeBinder TryGetBodyBinder(BinderFactory binderFactoryOpt = null,
            bool ignoreAccessibility = false)
        {
            return null;
        }
    }
    
    internal class PascalMethodSymbol : PascalMethodBaseSymbol
    {
        public override bool IsGenericMethod { get; }
        public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;
        protected override object MethodChecksLockObject { get; }
        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations { get; }

        private ImmutableArray<TypeParameterSymbol> _typeParameters;

        public PascalMethodSymbol(IMethodInfoAdapter method, NamedTypeSymbol containingType, bool isIterator) 
            : base(method, containingType, isIterator)
        {
            CreateTypeParameters(method);
            CreateParameters(method, this);

            _returnTypeWithAnnotations = TypeWithAnnotations.Create(ResolveHelper.ResolveType(method.ReturnType));

            IsGenericMethod = method.IsGenericMethod;
            MethodChecksLockObject = new object();

            if (method.IsAbstract)
            {
                DeclarationModifiers |= DeclarationModifiers.Abstract;
            }

            if (method.IsVirtual)
            {
                DeclarationModifiers |= DeclarationModifiers.Virtual;
                var baseMethod = ResolveHelper.ResolveMethod(method, containingType.BaseTypeNoUseSiteDiagnostics);
                if (baseMethod is object)
                {
                    DeclarationModifiers |= DeclarationModifiers.Override;
                }
            }

            var methodKind = MethodKind.Ordinary;
            if (method.Name.StartsWith("<>lambda")) methodKind = MethodKind.LambdaMethod;
            else if (method.Name.StartsWith("get_")) methodKind = MethodKind.PropertyGet;
            else if (method.Name.StartsWith("set_")) methodKind = MethodKind.PropertySet;
            else if (method.Name.StartsWith("op_")) methodKind = MethodKind.UserDefinedOperator;
            
            if (method.Name.Contains('.'))
            {
                methodKind = MethodKind.ExplicitInterfaceImplementation;
                var meth = method as RoslynMethodBuilderAdapter;
                var tmp = new List<MethodSymbol>();
                tmp.Add(ResolveHelper.ResolveMethod(meth.Overrides));
                ExplicitInterfaceImplementations = tmp.ToImmutableArray();
            }
            else
            {
                ExplicitInterfaceImplementations = ImmutableArray<MethodSymbol>.Empty;
            }

            MakeFlags(methodKind, DeclarationModifiers, method.ReturnType.Equals(typeof(void).GetAdapter()), false, false);
        }

        public override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            return _typeParameters
                .Select(_ => TypeParameterConstraintKind.ObliviousNullabilityIfReferenceType)
                .ToImmutableArray();
        }
        
        private void CreateTypeParameters(IMethodInfoAdapter method)
        {
            var typeParameters = method
                .GetGenericArguments()
                .Cast<RoslynGenericTypeParameterBuilderAdapter>()
                .Select(param => new PascalTypeParameterSymbol(param))
                .Cast<TypeParameterSymbol>()
                .ToImmutableArray();
            foreach (var typeParameter in typeParameters)
            {
                (typeParameter as PascalTypeParameterSymbol).SetContainingSymbol(this);
            }

            _typeParameters = typeParameters;
        }
    }

    class PascalConstructorSymbol: PascalMethodBaseSymbol
    {
        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;
        
        public override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            throw new System.NotImplementedException();
        }

        public override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            throw new NotImplementedException();
        }

        protected override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
            throw new System.NotImplementedException();
        }

        internal override ExecutableCodeBinder TryGetBodyBinder(BinderFactory binderFactoryOpt = null, bool ignoreAccessibility = false)
        {
            throw new System.NotImplementedException();
        }

        public PascalConstructorSymbol(IConstructorInfoAdapter ctor, NamedTypeSymbol declaringType)
            : base(ctor, declaringType, false)
        {
            CreateParameters(ctor, this);
            MakeFlags(ctor.IsStatic ? MethodKind.StaticConstructor : MethodKind.Constructor, DeclarationModifiers, false, false, false);
        }
    }
    
    internal class PascalPropertySymbol: PropertySymbol
    {
        public override Symbol ContainingSymbol { get; }
        public override ImmutableArray<Location> Locations => new List<Location> {NoLocation.Singleton}.ToImmutableArray();
        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences =>
            ImmutableArray<SyntaxReference>.Empty;
        public override Accessibility DeclaredAccessibility { get; }
        internal override ObsoleteAttributeData ObsoleteAttributeData => ObsoleteAttributeData.Uninitialized;
        public override bool IsStatic { get; }
        public override bool IsVirtual { get; }
        public override bool IsOverride { get; }
        public override bool IsAbstract { get; }
        public override bool IsSealed { get; }
        public override bool IsExtern { get; }
        public override RefKind RefKind { get; }
        public override TypeWithAnnotations TypeWithAnnotations { get; }
        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;
        public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;
        public override bool IsIndexer { get; }
        internal override bool IsRequired { get; }
        internal override bool HasSpecialName { get; }
        public override MethodSymbol GetMethod { get; }
        public override MethodSymbol SetMethod { get; }
        internal override CallingConvention CallingConvention { get; }
        internal override bool MustCallMethodsDirectly { get; }
        internal override bool HasUnscopedRefAttribute { get; }
        public override string Name { get; }
        public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations =>
            ImmutableArray<PropertySymbol>.Empty;
        private LexicalSortKey _sortKey;
        
        public PascalPropertySymbol(IPropertyInfoAdapter property, TypeSymbol containing, Symbol getMethod, Symbol setMethod)
        {
            ContainingSymbol = containing;
            CallingConvention = CallingConvention.HasThis;

            GetMethod = getMethod as MethodSymbol;
            SetMethod = setMethod as MethodSymbol;
            Name = property.Name;

            TypeWithAnnotations = TypeWithAnnotations.Create(ResolveHelper.ResolveType(property.PropertyType));
            
            if (property.IsPublic) DeclaredAccessibility = Accessibility.Public;
            else if (property.IsPrivate) DeclaredAccessibility = Accessibility.Private;
            else if (property.IsAssembly) DeclaredAccessibility = Accessibility.Internal;
            else if (property.IsFamily) DeclaredAccessibility = Accessibility.Protected;
            else if (property.IsFamilyAndAssembly) DeclaredAccessibility = Accessibility.ProtectedAndInternal;
            else if (property.IsFamilyOrAssembly) DeclaredAccessibility = Accessibility.ProtectedOrInternal;
            else DeclaredAccessibility = Accessibility.Private;
            
            
            _sortKey = new LexicalSortKey(0, LexicalSortCounter.Counter++);
        }
        
        internal override LexicalSortKey GetLexicalSortKey()
        {
            return _sortKey;
        }
    }

    class PascalFieldSymbol : SourceFieldSymbol
    {
        public override string Name { get; }
        public override ImmutableArray<Location> Locations => new List<Location> {NoLocation.Singleton}.ToImmutableArray();
        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;
        public override RefKind RefKind => RefKind.None;
        public override Symbol AssociatedSymbol { get; }
        
        internal override Location ErrorLocation => null;
        
        protected override DeclarationModifiers Modifiers => _modifiers;
        protected override SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList => new SyntaxList<AttributeListSyntax>();

        private TypeSymbol _type;
        private DeclarationModifiers _modifiers;
        private LexicalSortKey _sortKey;
        
        public PascalFieldSymbol(IFieldInfoAdapter field, SourceMemberContainerTypeSymbol containingType, string name, TypeSymbol type) : base(containingType)
        {
            Name = name;
            _type = type;
            _sortKey = new LexicalSortKey(0, LexicalSortCounter.Counter++);
            ResolveHelper.FixGenericTypeArgumentsIfNeeded(containingType, type);

            _modifiers = DeclarationModifiers.None;

            if (field.IsPublic) _modifiers |= DeclarationModifiers.Public;
            else if (field.IsPrivate) _modifiers |= DeclarationModifiers.Private;
            else if (field.IsFamily) _modifiers |= DeclarationModifiers.Protected;
            else if (field.IsAssembly) _modifiers |= DeclarationModifiers.Internal;
            else if (field.IsFamilyAndAssembly) _modifiers |= DeclarationModifiers.PrivateProtected;
            else if (field.IsFamilyOrAssembly) _modifiers |= DeclarationModifiers.ProtectedInternal;
            else _modifiers |= DeclarationModifiers.Private;

            if (field.IsStatic)
            {
                _modifiers |= DeclarationModifiers.Static;
            }
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            return TypeWithAnnotations.Create(_type);
        }

        internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress,
            bool earlyDecodingWellKnownAttributes)
        {
            return null;
        }

        internal override LexicalSortKey GetLexicalSortKey()
        {
            return _sortKey;
        }
    }

    class PascalTypeParameterSymbol : TypeParameterSymbol
    {
        public override Symbol ContainingSymbol => _containingSymbol;
        public override ImmutableArray<Location> Locations { get; }
        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences { get; }
        public override int Ordinal { get; }
        public override bool HasConstructorConstraint { get; }
        public override TypeParameterKind TypeParameterKind => _typeParameterKind;
        internal override bool? IsNotNullable { get; }
        public override bool HasReferenceTypeConstraint { get; }
        public override bool IsReferenceTypeFromConstraintTypes { get; }
        internal override bool? ReferenceTypeConstraintIsNullable { get; }
        public override bool HasNotNullConstraint { get; }
        public override bool HasValueTypeConstraint { get; }
        public override bool IsValueTypeFromConstraintTypes { get; }
        public override bool HasUnmanagedTypeConstraint { get; }
        public override VarianceKind Variance { get; }
        public override string Name { get; }

        private Symbol _containingSymbol;
        private TypeParameterKind _typeParameterKind;

        public PascalTypeParameterSymbol(RoslynGenericTypeParameterBuilderAdapter builder)
        {
            Ordinal = builder.GenericParameterPosition;
            Variance = VarianceKind.None;
            builder.SetSymbol(this);
            Name = builder.Name;

            try
            {
                if (builder.DeclaringAdapter is null)
                {
                    _containingSymbol = null;
                }
                else if (builder.DeclaringAdapter is ITypeAdapter type)
                {
                    _containingSymbol = ResolveHelper.ResolveType(type);
                    _typeParameterKind = TypeParameterKind.Type;
                }
                else if (builder.DeclaringAdapter is IMethodInfoAdapter method)
                {
                    _containingSymbol = ResolveHelper.ResolveMethod(method);
                    _typeParameterKind = TypeParameterKind.Method;
                }
            }
            catch (NullReferenceException)
            {
                _containingSymbol = null;
            }
        }

        public void SetContainingSymbol(Symbol symbol)
        {
            _containingSymbol = symbol;
            _typeParameterKind = symbol is TypeSymbol ? TypeParameterKind.Type : TypeParameterKind.Method;
        }
        
        internal override void EnsureAllConstraintsAreResolved()
        {
            Console.WriteLine("PascalTypeParameterSymbol.EnsureAllConstraintsAreResolved not implemented");
            //throw new System.NotImplementedException();
        }

        internal override ImmutableArray<TypeWithAnnotations> GetConstraintTypes(ConsList<TypeParameterSymbol> inProgress)
        {
            Console.WriteLine("PascalTypeParameterSymbol.GetConstraintTypes not implemented");
            return ImmutableArray<TypeWithAnnotations>.Empty;
            //throw new System.NotImplementedException();
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfaces(ConsList<TypeParameterSymbol> inProgress)
        {
            Console.WriteLine("PascalTypeParameterSymbol.GetInterfaces not implemented");
            return ImmutableArray<NamedTypeSymbol>.Empty;
            //throw new System.NotImplementedException();
        }

        internal override NamedTypeSymbol GetEffectiveBaseClass(ConsList<TypeParameterSymbol> inProgress)
        {
            Console.WriteLine("PascalTypeParameterSymbol.GetEffectiveBaseClass not implemented");
            return _containingSymbol.ContainingAssembly.GetType(typeof(object).GetAdapter()) as NamedTypeSymbol;
            //throw new System.NotImplementedException();
        }

        internal override TypeSymbol GetDeducedBaseType(ConsList<TypeParameterSymbol> inProgress)
        {
            Console.WriteLine("PascalTypeParameterSymbol.GetDeducedBaseType not implemented");
            return _containingSymbol.ContainingAssembly.GetType(typeof(object).GetAdapter()) as NamedTypeSymbol;
            //throw new System.NotImplementedException();
        }
    }

    static class SymbolExtensions
    {
        public static TypeSymbol GetType(this AssemblySymbol assembly, ITypeAdapter type)
        {
            if (type.IsGenericParameter)
            {
                var typeParameterBuilder = type as RoslynGenericTypeParameterBuilderAdapter ?? new RoslynGenericTypeParameterBuilderAdapter((ITypeAdapter)null, type.Name, 0);
                var symbol = typeParameterBuilder?.Symbol ?? new PascalTypeParameterSymbol(typeParameterBuilder);
                return symbol;
            }
            
            if (type.IsArray)
            {
                var elementType = GetType(assembly, type.GetElementType());
                var arrayType = TypeWithAnnotations.Create(false, elementType);
                return ArrayTypeSymbol.CreateCSharpArray(assembly, arrayType);
            }

            if (type.IsPointer)
            {
                var t = GetType(assembly, type.GetElementType());
                return new PointerTypeSymbol(TypeWithAnnotations.Create(t));
            }

            if (type.IsByRef)
            {
                return GetType(assembly, type.GetElementType());
            }
            
            return GetNamedType(assembly, type);
        }

        public static NamedTypeSymbol GetNamedType(this AssemblySymbol assembly, ITypeAdapter type)
        {
            if (type is RoslynGenericTypeAdapter generic)
            {
                return generic.Adaptee;
                //return generic.Adaptee.IsUnboundGenericType ? generic.Adaptee.ConstructedFrom : generic.Adaptee;
            }

            string name = "";
            if (type.FullName.EndsWith("&"))
            {
                name = type.FullName.Replace("&", "");
            }
            else
            {
                name = type.FullName;
            }

            if (type.IsGenericType)
            {
                name += $"`{type.GetGenericArguments().Length}";
            }
            
            var t = assembly.GetTypeByMetadataName(name, true, false, out _);
            if (t is null)
            {
                return null;
            }
            
            if (type.IsGenericType && !type.IsGenericTypeDefinition && !type.IsGenericParameter)
            {
                var types = type
                    .GetGenericArguments()
                    .Select(param => GetType(assembly, param))
                    .Select(param => TypeWithAnnotations.Create(param))
                    .ToImmutableArray();
                    
                t = t.ConstructIfGeneric(types);
            }

            return t;
        }
        
        public static SourceNamedTypeSymbol GetType(this NamespaceSymbol namespaceSymbol, ITypeAdapter type)
        {
            var currentNamespace = namespaceSymbol;
            if(type.Namespace.Contains(namespaceSymbol.QualifiedName) && type.Namespace != namespaceSymbol.QualifiedName)
            {
                var namespaces = type.Namespace.Split('.');
                currentNamespace = namespaces.Aggregate(currentNamespace, (current, ns) => current.GetMembers(ns).OfType<SourceNamespaceSymbol>().First());
            }
                
            return currentNamespace.GetMembers(type.Name).FirstOrDefault() as SourceNamedTypeSymbol;
        }
    }
}