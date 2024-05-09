using System.Linq;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkMethodInfoAdapter: FrameworkMemberInfoAdapter, IMethodInfoAdapter
    {
        public ITypeAdapter ReturnType => Adaptee.ReturnType.GetAdapter();
        public ITypeAdapter DeclaringType => Adaptee.DeclaringType.GetAdapter();
        public override string Name => Adaptee.Name;
        public int MetadataToken => Adaptee.MetadataToken;
        public bool IsStatic => Adaptee.IsStatic;
        public bool IsVirtual => Adaptee.IsVirtual;
        public bool IsAbstract => Adaptee.IsAbstract;
        public bool IsSpecialName => Adaptee.IsSpecialName;
        public override bool IsPublic => Adaptee.IsPublic;
        public override bool IsPrivate => Adaptee.IsPrivate;
        public bool IsGenericMethod => Adaptee.IsGenericMethod;
        public bool IsGenericMethodDefinition => Adaptee.IsGenericMethodDefinition;
        public MethodAttributes Attributes => Adaptee.Attributes;
        public new MethodInfo Adaptee { get; }

        public FrameworkMethodInfoAdapter(MethodInfo info): base(info)
        {
            Adaptee = info;
        }
        
        public IMethodInfoAdapter MakeGenericMethod(ITypeAdapter type)
        {
            return Adaptee.MakeGenericMethod((type as FrameworkTypeAdapter).Adaptee).GetAdapter();
        }

        public IMethodInfoAdapter MakeGenericMethod(ITypeAdapter[] types)
        {
            var ts = types.Select(type => (type as FrameworkTypeAdapter).Adaptee).ToArray();
            return Adaptee.MakeGenericMethod(ts).GetAdapter();
        }

        public object[] GetCustomAttributes(ITypeAdapter type, bool inherit)
        {
            return Adaptee.GetCustomAttributes((type as FrameworkTypeAdapter).Adaptee, inherit);
        }

        public IMethodInfoAdapter GetGenericMethodDefinition()
        {
            return Adaptee.GetGenericMethodDefinition().GetAdapter();
        }

        public IParameterInfoAdapter[] GetParameters()
        {
            return Adaptee.GetParameters().Select(p => p.GetAdapter()).ToArray();
        }

        public void Invoke(object obj, object[] parameters)
        {
            Adaptee.Invoke(obj, parameters);
        }

        public ITypeAdapter[] GetGenericArguments()
        {
            return Adaptee.GetGenericArguments().Select(elem => elem.GetAdapter()).ToArray();
        }
    }
}