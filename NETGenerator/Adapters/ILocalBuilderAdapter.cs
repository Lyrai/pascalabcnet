namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface ILocalBuilderAdapter: ILocalInfoAdapter
    {
        ITypeAdapter LocalType { get; }

        void SetLocalSymInfo(string name);
    }
}