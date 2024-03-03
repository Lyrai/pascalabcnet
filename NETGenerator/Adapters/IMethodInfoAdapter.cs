namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IMethodInfoAdapter: IMemberInfoAdapter
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
        
        IMethodInfoAdapter MakeGenericMethod(ITypeAdapter type);
        IMethodInfoAdapter MakeGenericMethod(ITypeAdapter[] types);
        object[] GetCustomAttributes(ITypeAdapter type, bool inherit);
        IMethodInfoAdapter GetGenericMethodDefinition();
        IParameterInfoAdapter[] GetParameters();
        void Invoke(object obj, object[] parameters);
    }
}