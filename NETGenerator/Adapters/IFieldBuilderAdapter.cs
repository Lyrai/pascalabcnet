namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IFieldBuilderAdapter: IFieldInfoAdapter
    {
        void SetConstant(object value);
        void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute);
    }
}