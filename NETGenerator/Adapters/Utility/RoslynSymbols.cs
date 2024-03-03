using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.Cci;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslyn.Utilities;
using System.Linq;
using BindingDiagnosticBag = Microsoft.CodeAnalysis.CSharp.BindingDiagnosticBag;
using FieldSymbol = Microsoft.CodeAnalysis.CSharp.Symbols.FieldSymbol;
using NamedTypeSymbol = Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol;
using ParameterSymbol = Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol;
using Symbol = Microsoft.CodeAnalysis.CSharp.Symbol;
using TypeParameterSymbol = Microsoft.CodeAnalysis.CSharp.Symbols.TypeParameterSymbol;
using TypeSymbol = Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    internal class PascalMethodSymbol : SourceMemberMethodSymbol
    {
        public PascalMethodSymbol(NamedTypeSymbol containingType, bool isIterator, TypeSymbol returnType) 
            : base(containingType, null, null, isIterator)
        {
            _parameterSymbols = ImmutableArray<ParameterSymbol>.Empty;
            _returnType = TypeWithAnnotations.Create(returnType);
            //_sortKey = new LexicalSortKey(0, n.Token.Line);
            //Name = n.Name;
        }

        internal override LexicalSortKey GetLexicalSortKey()
        {
            return _sortKey;
        }

        public override Accessibility DeclaredAccessibility => Accessibility.Public;

        public override string Name { get; }

        public override bool IsVararg => false;

        public override RefKind RefKind => RefKind.None;

        public override TypeWithAnnotations ReturnTypeWithAnnotations => _returnType;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        public override ImmutableArray<ParameterSymbol> Parameters => _parameterSymbols;

        internal override bool GenerateDebugInfo => false;
        
        internal override bool IsExpressionBodied => false;

        private ImmutableArray<ParameterSymbol> _parameterSymbols;
        private TypeWithAnnotations _returnType;
        private LexicalSortKey _sortKey;

        public override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            return ImmutableArray<ImmutableArray<TypeWithAnnotations>>.Empty;
        }

        public override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            return ImmutableArray<TypeParameterConstraintKind>.Empty;
        }

        protected override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
        }

        internal override ExecutableCodeBinder TryGetBodyBinder(BinderFactory binderFactoryOpt = null,
            bool ignoreAccessibility = false)
        {
            return null;
        }

        public void SetParameters(ImmutableArray<ParameterSymbol> parameters)
        {
            _parameterSymbols = parameters;
        }

        public void SetStatic()
        {
            DeclarationModifiers |= DeclarationModifiers.Static;
        }
    }

    class PascalParameterSymbol : ParameterSymbol
    {
        public override Symbol ContainingSymbol { get; }

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override TypeWithAnnotations TypeWithAnnotations { get; }

        public override RefKind RefKind => RefKind.None;

        public override bool IsDiscard => false;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        internal override MarshalPseudoCustomAttributeData MarshallingInformation => null;

        public override int Ordinal { get; }

        public override bool IsParams => false;

        internal override bool IsMetadataOptional => false;

        internal override bool IsMetadataIn => false;

        internal override bool IsMetadataOut => false;

        internal override ConstantValue ExplicitDefaultConstantValue => null;

        internal override bool IsIDispatchConstant => false;

        internal override bool IsIUnknownConstant => false;

        internal override bool IsCallerFilePath => false;

        internal override bool IsCallerLineNumber => false;

        internal override bool IsCallerMemberName => false;

        internal override int CallerArgumentExpressionParameterIndex => Ordinal;

        internal override FlowAnalysisAnnotations FlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        internal override ImmutableHashSet<string> NotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

        internal override ImmutableArray<int> InterpolatedStringHandlerArgumentIndexes => ImmutableArray<int>.Empty;

        internal override bool HasInterpolatedStringHandlerArgumentError => false;

        internal override ScopedKind EffectiveScope => ScopedKind.None;

        internal override bool HasUnscopedRefAttribute => false;

        internal override bool UseUpdatedEscapeRules => false;

        public PascalParameterSymbol(Symbol containingSymbol, int ordinal, TypeSymbol type)
        {
            ContainingSymbol = containingSymbol;
            Ordinal = ordinal;
            TypeWithAnnotations = TypeWithAnnotations.Create(type);
        }
    }

    class PascalFieldSymbol : SourceFieldSymbol
    {
        public PascalFieldSymbol(SourceMemberContainerTypeSymbol containingType, string name, TypeSymbol type) : base(containingType)
        {
            Name = name;
            _type = type;
        }

        public override string Name { get; }

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override RefKind RefKind => RefKind.None;

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            return TypeWithAnnotations.Create(_type);
        }

        public override Symbol AssociatedSymbol { get; }

        internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress,
            bool earlyDecodingWellKnownAttributes)
        {
            return null;
        }

        internal override Location ErrorLocation => null;

        protected override DeclarationModifiers Modifiers => DeclarationModifiers.Public;

        protected override SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList => new SyntaxList<AttributeListSyntax>();

        private TypeSymbol _type;
    }

    static class SymbolExtensions
    {
        public static NamedTypeSymbol GetType(this AssemblySymbol assembly, ITypeAdapter type)
        {
            return assembly.GetTypeByMetadataName(type.FullName, true, false, out _);
        }
        
        public static SourceNamedTypeSymbol GetType(this NamespaceSymbol namespaceSymbol, ITypeAdapter type)
        {
            var namespaces = type.Namespace.Split('.');
            var currentNamespace = namespaceSymbol;
            foreach (var ns in namespaces)
            {
                currentNamespace = currentNamespace.GetMembers(ns).First() as SourceNamespaceSymbol;
            }
                
            return currentNamespace.GetMembers(type.Name).First() as SourceNamedTypeSymbol;
        }
    }
}