namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IFieldInfoAdapter: IMemberInfoAdapter
    {
        ITypeAdapter FieldType { get; }
        bool IsLiteral { get; }
        bool IsStatic { get; }
        string Name { get; }

        object GetRawConstantValue();
    }
}