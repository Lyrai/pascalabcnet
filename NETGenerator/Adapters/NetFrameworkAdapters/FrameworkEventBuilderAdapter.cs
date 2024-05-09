using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
#if !NETCOREAPP
    public class FrameworkEventBuilderAdapter: IEventBuilderAdapter
    {
        public EventBuilder Adaptee { get; }

        public FrameworkEventBuilderAdapter(EventBuilder builder)
        {
            Adaptee = builder;
        }
        
        public void SetAddOnMethod(IMethodBuilderAdapter method)
        {
            Adaptee.SetAddOnMethod((method as FrameworkMethodBuilderAdapter).Adaptee);
        }

        public void SetRemoveOnMethod(IMethodBuilderAdapter method)
        {
            Adaptee.SetRemoveOnMethod((method as FrameworkMethodBuilderAdapter).Adaptee);
        }

        public void SetRaiseMethod(IMethodBuilderAdapter method)
        {
            Adaptee.SetRaiseMethod((method as FrameworkMethodBuilderAdapter).Adaptee);
        }
        
        public override int GetHashCode()
        {
            return Adaptee.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FrameworkEventBuilderAdapter adapter))
            {
                return false;
            }

            return Adaptee.Equals(adapter.Adaptee);
        }
    }
#endif
}