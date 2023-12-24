using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkMemberInfoAdapter: IMemberInfoAdapter
    {
        public  MemberInfo Adaptee { get; }

        public FrameworkMemberInfoAdapter(MemberInfo info)
        {
            Adaptee = info;
        }
        
        public override int GetHashCode()
        {
            return Adaptee.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FrameworkMemberInfoAdapter info))
            {
                return false;
            }

            return Adaptee.Equals(info.Adaptee);
        }
    }
}