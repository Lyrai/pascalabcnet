namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynLocalBuilderAdapter: ILocalBuilderAdapter
    {
        public ITypeAdapter LocalType { get; }
        public bool Pinned { get; }

        public RoslynLocalBuilderAdapter(ITypeAdapter localType, bool pinned)
        {
            LocalType = localType;
            Pinned = pinned;
        }

        public void SetLocalSymInfo(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}