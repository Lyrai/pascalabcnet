using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using PascalABCCompiler.NETGenerator.Adapters;
using Roslyn.Utilities;

namespace NETGenerator.Adapters.Utility
{
    internal static class ResolveHelper
    {
        private static AssemblySymbol _assembly;

        public static void Init(AssemblySymbol assemblySymbol)
        {
            _assembly = assemblySymbol;
        }

        public static TypeSymbol ResolveType(ITypeAdapter type)
        {
            return _assembly.GetType(type);
        }

        public static NamedTypeSymbol ResolveNamedType(ITypeAdapter type)
        {
            return _assembly.GetNamedType(type);
        }

        public static MethodSymbol ResolveMethod(IMethodBaseAdapter methodInfo)
        {
            var argumentTypes = GetParameterTypes(methodInfo);
            
            var declaringType = methodInfo.DeclaringType;
            var methodName = methodInfo.Name;

            var typeSymbol = ResolveType(declaringType);
            MethodSymbol methodSymbol;

            do
            {
                methodSymbol = FindMethodInType(typeSymbol, methodName, argumentTypes);
                typeSymbol = typeSymbol.BaseTypeNoUseSiteDiagnostics;
            } while (methodSymbol is null);

            if (!methodSymbol.IsGenericMethod)
            {
                return methodSymbol;
            }
            
            var typeParameterSymbols = methodInfo
                .GetGenericArguments()
                .Select(ResolveType)
                .Select(param => TypeWithAnnotations.Create(param))
                .ToImmutableArray();
            
            return methodSymbol.ConstructIfGeneric(typeParameterSymbols);
        }

        public static MethodSymbol ResolveMethodInType(TypeSymbol declaringType, IMethodBaseAdapter method)
        {
            return FindMethodInType(declaringType, method.Name, GetParameterTypes(method));
        }

        public static FieldSymbol ResolveField(IFieldInfoAdapter field)
        {
            var netType = ResolveType(field.DeclaringType);
            return netType.GetMembers(field.Name).OfType<FieldSymbol>().First();
        }
        
        private static bool SelectMethodByParamsExact(Symbol symbol, ITypeAdapter[] argumentTypes)
        {
            var parameters = symbol.GetParameters();
            var parameterTypes = parameters.Select(x => x.Type);
            var args = argumentTypes.Select(ResolveType);

            return parameterTypes
                .Zip(args, (param, arg) => (param, arg))
                .All(tuple => tuple.param.MetadataName == tuple.arg.MetadataName);
        }

        private static bool SelectMethodByParamsWithCastChecks(Symbol symbol, ITypeAdapter[] argumentTypes)
        {
            var parameters = symbol.GetParameters();
            var parameterTypes = parameters.Select(x => x.Type).ToArray();
            var useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>();
                    
            return parameterTypes
                .Zip(
                    argumentTypes, 
                    (param, arg) => (param, arg: ResolveType(arg))
                )
                .All(t => t.arg.CanUnifyWith(t.param) || t.arg.IsEqualToOrDerivedFrom(t.param, TypeCompareKind.ConsiderEverything, ref useSiteInfo));
        }

        private static MethodSymbol FindMethodInType(TypeSymbol type, string methodName, ITypeAdapter[] argumentTypes)
        {
            var members = type
                .GetMembers(methodName)
                .Where(symbol => symbol.GetParameters().Length == argumentTypes.Length)
                .ToArray();

            if (members.IsEmpty())
            {
                return null;
            }

            return (members.FirstOrDefault(symbol => SelectMethodByParamsExact(symbol, argumentTypes))
                    ?? members.FirstOrDefault(symbol => SelectMethodByParamsWithCastChecks(symbol, argumentTypes))) as MethodSymbol;
        }

        private static ITypeAdapter[] GetParameterTypes(IMethodBaseAdapter method)
        {
            return method
                .GetParameters()
                .Select(param => param.ParameterType)
                .ToArray();
        }
    }
}