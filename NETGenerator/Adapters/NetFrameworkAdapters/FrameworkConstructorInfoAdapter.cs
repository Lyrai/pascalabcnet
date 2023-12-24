using System.Linq;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkConstructorInfoAdapter: IConstructorInfoAdapter
    {
        public ITypeAdapter DeclaringType => Adaptee.DeclaringType.GetAdapter();
        
        public ConstructorInfo Adaptee { get; }

        public FrameworkConstructorInfoAdapter(ConstructorInfo info)
        {
            Adaptee = info;
        }
        
        public IParameterInfoAdapter[] GetParameters()
        {
            return Adaptee.GetParameters().Select(p => p.GetAdapter()).ToArray();
        }
    }
}