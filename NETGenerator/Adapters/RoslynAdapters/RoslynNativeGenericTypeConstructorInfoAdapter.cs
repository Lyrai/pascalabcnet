using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using NETGenerator.Adapters.Utility;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynNativeGenericTypeConstructorInfoAdapter: FrameworkConstructorInfoAdapter
    {
        public override ITypeAdapter DeclaringType { get; }
        public MethodSymbol Symbol { get; }

        public RoslynNativeGenericTypeConstructorInfoAdapter(ConstructorInfo info, RoslynGenericTypeAdapter declaringType) : base(info)
        {
            DeclaringType = declaringType;
            //Symbol = ResolveHelper.ResolveMethod(info.GetAdapter(), declaringType.Adaptee);
        }
    }
}