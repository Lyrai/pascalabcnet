namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IFieldBuilderAdapter: IFieldInfoAdapter
    {
        bool IsStatic { get; }

        void SetConstant(object value);
        void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute);
    }
}