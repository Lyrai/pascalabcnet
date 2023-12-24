namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IModuleAdapter: IAdapter
    {
        ITypeAdapter GetType(string name);
    }
}