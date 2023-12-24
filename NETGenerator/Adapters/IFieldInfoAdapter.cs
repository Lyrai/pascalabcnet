namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IFieldInfoAdapter: IMemberInfoAdapter
    {
        ITypeAdapter FieldType { get; }
        bool IsLiteral { get; }

        object GetRawConstantValue();
    }
}