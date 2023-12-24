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
        
        public override int GetHashCode()
        {
            return Adaptee.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FrameworkParameterInfoAdapter info))
            {
                return false;
            }

            return Adaptee.Equals(info.Adaptee);
        }
    }
}