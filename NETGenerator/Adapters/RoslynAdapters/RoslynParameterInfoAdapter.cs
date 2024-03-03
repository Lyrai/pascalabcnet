namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynParameterInfoAdapter: IParameterInfoAdapter
    {
        public ITypeAdapter ParameterType { get; }

        public RoslynParameterInfoAdapter(ITypeAdapter type)
        {
            ParameterType = type;
        }
    }
}