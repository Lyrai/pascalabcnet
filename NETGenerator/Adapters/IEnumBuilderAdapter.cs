namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IEnumBuilderAdapter: ITypeAdapter
    {
        ITypeAdapter CreateType();
        void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute);
        void DefineLiteral(string name, object value);
    }
}