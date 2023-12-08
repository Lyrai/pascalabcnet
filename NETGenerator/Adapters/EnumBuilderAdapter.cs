namespace PascalABCCompiler.NETGenerator.Adapters
{
    public abstract class EnumBuilderAdapter: TypeAdapter
    {
        public abstract TypeAdapter CreateType();
        public abstract void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute);
        public abstract void DefineLiteral(string name, object value);
    }
}