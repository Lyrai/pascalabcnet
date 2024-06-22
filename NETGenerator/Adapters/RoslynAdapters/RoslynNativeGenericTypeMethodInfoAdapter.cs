using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using NETGenerator.Adapters.Utility;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynNativeGenericTypeMethodInfoAdapter: FrameworkMethodInfoAdapter
    {
        public override ITypeAdapter DeclaringType { get; }
        private ITypeAdapter _declaringType;

        public RoslynNativeGenericTypeMethodInfoAdapter(MethodInfo info, ITypeAdapter declaringType) 
            :base(info)
        {
            DeclaringType = declaringType;
            //Symbol = ResolveHelper.ResolveMethod(info.GetAdapter(), declaringType.Adaptee);
        }
    }
}