using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkLocalBuilderAdapter: ILocalBuilderAdapter
    {
        public ITypeAdapter LocalType => Adaptee.LocalType.GetAdapter();
        public LocalBuilder Adaptee { get; }

        public FrameworkLocalBuilderAdapter(LocalBuilder adaptee)
        {
            Adaptee = adaptee;
        }

        public void SetLocalSymInfo(string name)
        {
            Adaptee.SetLocalSymInfo(name);
        }
    }
}