using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkEventBuilderAdapter: IEventBuilderAdapter
    {
        private EventBuilder Adaptee { get; }

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
    }
}