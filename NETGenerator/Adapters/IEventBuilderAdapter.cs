namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IEventBuilderAdapter: IAdapter
    {
        void SetAddOnMethod(IMethodBuilderAdapter method);
        void SetRemoveOnMethod(IMethodBuilderAdapter method);
        void SetRaiseMethod(IMethodBuilderAdapter method);
    }
}