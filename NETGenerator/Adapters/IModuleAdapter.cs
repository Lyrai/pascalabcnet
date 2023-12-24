namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IModuleAdapter
    {
        ITypeAdapter GetType(string name);
    }
}