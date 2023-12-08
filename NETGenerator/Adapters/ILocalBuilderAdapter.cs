namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface ILocalBuilderAdapter: ILocalInfoAdapter
    {
        TypeAdapter LocalType { get; }

        void SetLocalSymInfo(string name);
    }
}