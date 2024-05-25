using System.Reflection;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynNativeGenericTypeMethodInfoAdapter: FrameworkMethodInfoAdapter
    {
        public override ITypeAdapter DeclaringType => _declaringType;
        private ITypeAdapter _declaringType;

        public RoslynNativeGenericTypeMethodInfoAdapter(MethodInfo info, ITypeAdapter declaringType) : base(info)
        {
            _declaringType = declaringType;
        }
    }
}