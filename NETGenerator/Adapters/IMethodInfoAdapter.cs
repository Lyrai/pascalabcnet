namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IMethodInfoAdapter: IMemberInfoAdapter
    {
        TypeAdapter ReturnType { get; }
        TypeAdapter DeclaringType { get; }
        string Name { get; }
        int MetadataToken { get; }
        bool IsStatic { get; }
        bool IsVirtual { get; }
        bool IsAbstract { get; }
        
        IMethodInfoAdapter MakeGenericMethod(TypeAdapter type);
        IMethodInfoAdapter MakeGenericMethod(TypeAdapter[] types);
        IMethodInfoAdapter GetGenericMethodDefinition();
        IParameterInfoAdapter[] GetParameters();
        void Invoke(object obj, object[] parameters);
    }
}