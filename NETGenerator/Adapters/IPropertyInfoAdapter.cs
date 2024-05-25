namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IPropertyInfoAdapter: IMemberInfoAdapter
    {
        ITypeAdapter PropertyType { get; }
        
        IMethodInfoAdapter GetGetMethod();
        IMethodInfoAdapter GetSetMethod();
    }
}