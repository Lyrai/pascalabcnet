using System.Linq;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkConstructorInfoAdapter: FrameworkMemberInfoAdapter, IConstructorInfoAdapter
    {
        public ITypeAdapter DeclaringType => Adaptee.DeclaringType.GetAdapter();
        
        public new ConstructorInfo Adaptee { get; }

        public FrameworkConstructorInfoAdapter(ConstructorInfo info): base(info)
        {
            Adaptee = info;
        }
        
        public IParameterInfoAdapter[] GetParameters()
        {
            return Adaptee.GetParameters().Select(p => p.GetAdapter()).ToArray();
        }
    }
}