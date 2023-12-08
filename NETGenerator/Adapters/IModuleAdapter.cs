namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IModuleAdapter
    {
        TypeAdapter GetType(string name);
    }
}