using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkParameterInfoAdapter: IParameterInfoAdapter
    {
        public ITypeAdapter ParameterType => Adaptee.ParameterType.GetAdapter();
        
        public ParameterInfo Adaptee { get; }

        public FrameworkParameterInfoAdapter(ParameterInfo info)
        {
            Adaptee = info;
        }
    }
}