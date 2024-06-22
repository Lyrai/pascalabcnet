using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using PascalABCCompiler.NETGenerator.Adapters;
using PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters;
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

        public static MethodSymbol ResolveMethod(IMethodBaseAdapter methodInfo, TypeSymbol declaringType = null)
        {
            if (methodInfo is RoslynGenericMethodInfoAdapter genericMethod)
            {
                return genericMethod.Adaptee;
            }
            
            var argumentTypes = GetParameterTypes(methodInfo);
            
            if(declaringType is null)
            {
                declaringType = ResolveType(methodInfo.DeclaringType);
            }
            
            if (declaringType.IsUnboundGenericType())
            {
                declaringType = declaringType.ConstructedFrom() as TypeSymbol;
            }
            
            MethodSymbol methodSymbol;

            do
            {
                if (declaringType is null)
                {
                    return null;
                }
                methodSymbol = FindMethodInType(declaringType, methodInfo, argumentTypes);
                declaringType = declaringType.BaseTypeNoUseSiteDiagnostics;
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
            return FindMethodInType(declaringType, method, GetParameterTypes(method));
        }

        public static FieldSymbol ResolveField(IFieldInfoAdapter field, TypeSymbol declaringType = null)
        {
            var netType = declaringType ?? ResolveType(field.DeclaringType);
            return netType.GetMembers(field.Name).OfType<FieldSymbol>().First();
        }
        
        public static void FixGenericTypeArgumentsIfNeeded(Symbol containingSymbol, TypeSymbol symbol)
        {
            if (symbol is ArrayTypeSymbol arr)
            {
                FixGenericTypeArgumentsIfNeeded(containingSymbol, arr.ElementType);
            }

            if (symbol is PointerTypeSymbol ptr)
            {
                FixGenericTypeArgumentsIfNeeded(containingSymbol, ptr.PointedAtType);
                return;
            }

            if (symbol is PascalTypeParameterSymbol pascalTypeParameter)
            {
                if(pascalTypeParameter.ContainingSymbol is null)
                {
                    containingSymbol = FindTypeParameter(containingSymbol, pascalTypeParameter.Name).ContainingSymbol;
                    pascalTypeParameter.SetContainingSymbol(containingSymbol);
                }
                
                return;
            }
            
            if (!(symbol is NamedTypeSymbol namedType))
            {
                return;
            }

            if (!namedType.IsGenericType || namedType.IsUnboundGenericType)
                return;
            
            bool flag = false;
            foreach (var typeArg in namedType.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics)
            {
                FixGenericTypeArgumentsIfNeeded(containingSymbol, typeArg.Type);
            }
        }

        private static TypeSymbol FindTypeParameter(Symbol containing, string name)
        {
            TypeSymbol parameter = null;
            if(containing is MethodSymbol method)
            {
                parameter = method
                    .TypeParameters
                    .FirstOrDefault(methParam => methParam.Name == name);
                containing = method.ContainingSymbol;
            }

            if (parameter is null && containing is NamedTypeSymbol type)
            {
                parameter = type
                    .TypeParameters
                    .FirstOrDefault(typeParameter => typeParameter.Name == name);
            }

            Debug.Assert(parameter is TypeParameterSymbol);
            return parameter;
        }
        
        private static bool SelectMethodByParamsExact(Symbol symbol, ITypeAdapter[] argumentTypes)
        {
            var parameters = symbol.GetParameters();
            var parameterTypes = parameters.Select(x => x.Type);
            var args = argumentTypes.Select(ResolveType);

            return parameterTypes
                .Zip(args, (param, arg) => (param, arg))
                .All(tuple => tuple.param.Equals(tuple.arg));
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

        private static MethodSymbol FindMethodInType(TypeSymbol type, IMethodBaseAdapter method, ITypeAdapter[] argumentTypes)
        {
            /*if (type.IsUnboundGenericType())
            {
                type = type.ConstructedFrom() as TypeSymbol;
            }*/
            
            var members = type
                .GetMembers(method.Name)
                .OfType<MethodSymbol>()
                .Where(symbol => symbol.GetParameters().Length == argumentTypes.Length)
                .ToArray();

            if (members.IsEmpty())
            {
                return null;
            }

            var exactMatch = members.FirstOrDefault(symbol => SelectMethodByParamsExact(symbol, argumentTypes));
            if (exactMatch is object)
            {
                return exactMatch as MethodSymbol;
            }

            if (!(method is IMethodInfoAdapter methodInfo) || !methodInfo.IsGenericMethod)
            {
                return members.FirstOrDefault(symbol => SelectMethodByParamsWithCastChecks(symbol, argumentTypes)) as MethodSymbol;
            }

            return members
                .Cast<MethodSymbol>()
                .Where(symbol => symbol.IsGenericMethod)
                .FirstOrDefault(symbol => SelectMethodByParamsWithCastChecks(symbol, argumentTypes));
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