using System;
using System.Collections.Generic;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public abstract class FrameworkMemberInfoAdapter: IMemberInfoAdapter
    {
        public abstract string Name { get; }
        public abstract bool IsPublic { get; }
        public abstract bool IsPrivate { get; }
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

        public IMemberInfoAdapter Instantiate(Dictionary<ITypeAdapter, ITypeAdapter> typeArguments,
            ITypeAdapter declaringType)
        {
            throw new NotSupportedException();
        }
    }
}