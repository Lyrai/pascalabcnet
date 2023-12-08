namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IConstructorInfoAdapter
    {
        TypeAdapter DeclaringType { get; }
        
        IParameterInfoAdapter[] GetParameters();
    }
}