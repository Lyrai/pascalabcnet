namespace NETGenerator.Adapters
{
    public interface IFieldBuilderAdapter: IFieldInfoAdapter
    {
        bool IsStatic { get; }
    }
}