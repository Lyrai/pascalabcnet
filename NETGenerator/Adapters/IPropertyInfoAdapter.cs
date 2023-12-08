namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IPropertyInfoAdapter: IMemberInfoAdapter
    {
        IMethodInfoAdapter GetGetMethod();
    }
}