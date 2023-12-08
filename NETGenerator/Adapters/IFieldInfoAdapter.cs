namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IFieldInfoAdapter
    {
        TypeAdapter FieldType { get; }
        bool IsLiteral { get; }

        object GetRawConstantValue();
    }
}