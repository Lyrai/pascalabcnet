using System.Reflection;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynNativeGenericTypeConstructorInfoAdapter: FrameworkConstructorInfoAdapter
    {
        public override ITypeAdapter DeclaringType => _declaringType;

        private ITypeAdapter _declaringType;
        public RoslynNativeGenericTypeConstructorInfoAdapter(ConstructorInfo info, ITypeAdapter declaringType) : base(info)
        {
            _declaringType = declaringType;
        }
    }
}