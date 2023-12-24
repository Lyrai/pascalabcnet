using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkFieldInfoAdapter: FrameworkMemberInfoAdapter, IFieldInfoAdapter
    {
        public ITypeAdapter FieldType => Adaptee.FieldType.GetAdapter();
        public bool IsLiteral => Adaptee.IsLiteral;
        public FieldInfo Adaptee { get; }

        public FrameworkFieldInfoAdapter(FieldInfo info)
        {
            Adaptee = info;
        }
        
        public object GetRawConstantValue()
        {
            return Adaptee.GetRawConstantValue();
        }
    }
}