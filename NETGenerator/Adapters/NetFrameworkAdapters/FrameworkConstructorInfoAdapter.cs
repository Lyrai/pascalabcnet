using System.Linq;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkConstructorInfoAdapter: FrameworkMemberInfoAdapter, IConstructorInfoAdapter
    {
        public ITypeAdapter DeclaringType => Adaptee.DeclaringType.GetAdapter();
        public override bool IsPrivate => Adaptee.IsPrivate;
        public override bool IsPublic => Adaptee.IsPublic;
        public override string Name => Adaptee.Name;
        public bool IsStatic => Adaptee.IsStatic;
        public ITypeAdapter ReturnType => Adaptee.DeclaringType.GetAdapter();
        public MethodAttributes Attributes => Adaptee.Attributes;

        public new ConstructorInfo Adaptee { get; }

        public FrameworkConstructorInfoAdapter(ConstructorInfo info): base(info)
        {
            Adaptee = info;
        }

        public IParameterInfoAdapter[] GetParameters()
        {
            return Adaptee.GetParameters().Select(p => p.GetAdapter()).ToArray();
        }
        
        public ITypeAdapter[] GetGenericArguments()
        {
            return Adaptee.GetGenericArguments().Select(arg => arg.GetAdapter()).ToArray();
        }
    }
}