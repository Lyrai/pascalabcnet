using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IMethodBaseAdapter : IMemberInfoAdapter
    {
        ITypeAdapter ReturnType { get; }
        IParameterInfoAdapter[] GetParameters();
        bool IsStatic { get; }
        MethodAttributes Attributes { get; }
        ITypeAdapter DeclaringType { get; }

        ITypeAdapter[] GetGenericArguments();
    }
}