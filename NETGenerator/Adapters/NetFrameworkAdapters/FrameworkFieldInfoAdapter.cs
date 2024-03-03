using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkFieldInfoAdapter: FrameworkMemberInfoAdapter, IFieldInfoAdapter
    {
        public ITypeAdapter FieldType => Adaptee.FieldType.GetAdapter();
        public bool IsLiteral => Adaptee.IsLiteral;
        public bool IsStatic => Adaptee.IsStatic;
        public string Name => Adaptee.Name;
        public new FieldInfo Adaptee { get; }

        public FrameworkFieldInfoAdapter(FieldInfo info): base(info)
        {
            Adaptee = info;
        }
        
        public object GetRawConstantValue()
        {
            return Adaptee.GetRawConstantValue();
        }
    }
}