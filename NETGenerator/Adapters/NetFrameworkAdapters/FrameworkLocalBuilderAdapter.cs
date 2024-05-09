using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
#if !NETCOREAPP
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
        
        public override int GetHashCode()
        {
            return Adaptee.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FrameworkLocalBuilderAdapter builder))
            {
                return false;
            }

            return Adaptee.Equals(builder.Adaptee);
        }
    }
#endif
}