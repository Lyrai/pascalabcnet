using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkPropertyInfoAdapter: IPropertyInfoAdapter
    {
        public PropertyInfo Adaptee { get; }

        public FrameworkPropertyInfoAdapter(PropertyInfo info)
        {
            Adaptee = info;
        }
        
        public IMethodInfoAdapter GetGetMethod()
        {
            return Adaptee.GetGetMethod().GetAdapter();
        }
    }
}