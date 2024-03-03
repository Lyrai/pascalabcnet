namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IModuleAdapter: IAdapter
    {
        AssemblyAdapter Assembly { get; }
        ITypeAdapter GetType(string name);
    }
}