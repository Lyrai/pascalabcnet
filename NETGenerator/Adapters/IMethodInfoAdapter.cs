namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IMethodInfoAdapter: IMethodBaseAdapter
    {
        ITypeAdapter ReturnType { get; }
        ITypeAdapter DeclaringType { get; }
        string Name { get; }
        int MetadataToken { get; }
        bool IsStatic { get; }
        bool IsVirtual { get; }
        bool IsAbstract { get; }
        bool IsSpecialName { get; }
        bool IsPublic { get; }
        bool IsPrivate { get; }
        bool IsGenericMethod { get; }
        bool IsGenericMethodDefinition { get; }
        
        IMethodInfoAdapter MakeGenericMethod(params ITypeAdapter[] types);
        object[] GetCustomAttributes(ITypeAdapter type, bool inherit);
        IMethodInfoAdapter GetGenericMethodDefinition();
        void Invoke(object obj, object[] parameters);
    }
}