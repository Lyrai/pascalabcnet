namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IPropertyBuilderAdapter: IPropertyInfoAdapter
    {
        void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute);
        void SetGetMethod(IMethodBuilderAdapter method);
        void SetSetMethod(IMethodBuilderAdapter method);
    }
}