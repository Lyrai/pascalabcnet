namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IParameterInfoAdapter: IAdapter
    {
        ITypeAdapter ParameterType { get; }
    }
}