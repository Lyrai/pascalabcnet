namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IConstructorInfoAdapter: IMemberInfoAdapter
    {
        ITypeAdapter DeclaringType { get; }
        
        IParameterInfoAdapter[] GetParameters();
    }
}