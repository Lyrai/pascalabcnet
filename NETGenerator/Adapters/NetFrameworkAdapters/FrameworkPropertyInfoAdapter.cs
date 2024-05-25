using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkPropertyInfoAdapter: FrameworkMemberInfoAdapter, IPropertyInfoAdapter
    {
        public override string Name => Adaptee.Name;
        public override bool IsPublic => true;
        public override bool IsPrivate => false;
        public override bool IsAssembly => false;
        public override bool IsFamily => false;
        public override bool IsFamilyAndAssembly => false;
        public override bool IsFamilyOrAssembly => false;
        public ITypeAdapter PropertyType => Adaptee.PropertyType.GetAdapter();
        public new PropertyInfo Adaptee { get; }

        public FrameworkPropertyInfoAdapter(PropertyInfo info): base(info)
        {
            Adaptee = info;
        }

        public IMethodInfoAdapter GetGetMethod()
        {
            return Adaptee.GetGetMethod().GetAdapter();
        }

        public IMethodInfoAdapter GetSetMethod()
        {
            return Adaptee.GetSetMethod().GetAdapter();
        }
    }
}