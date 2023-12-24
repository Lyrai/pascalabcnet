namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IEventBuilderAdapter
    {
        void SetAddOnMethod(IMethodBuilderAdapter method);
        void SetRemoveOnMethod(IMethodBuilderAdapter method);
        void SetRaiseMethod(IMethodBuilderAdapter method);
    }
}