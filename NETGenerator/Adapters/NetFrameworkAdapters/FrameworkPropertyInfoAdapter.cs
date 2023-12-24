using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkPropertyInfoAdapter: FrameworkMemberInfoAdapter, IPropertyInfoAdapter
    {
        public new PropertyInfo Adaptee { get; }

        public FrameworkPropertyInfoAdapter(PropertyInfo info): base(info)
        {
            Adaptee = info;
        }
        
        public IMethodInfoAdapter GetGetMethod()
        {
            return Adaptee.GetGetMethod().GetAdapter();
        }
    }
}