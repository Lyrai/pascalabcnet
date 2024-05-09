namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IConstructorInfoAdapter: IMethodBaseAdapter
    {
        ITypeAdapter DeclaringType { get; }
        bool IsPrivate { get; }
        bool IsPublic { get; }
    }
}