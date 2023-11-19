namespace NETGenerator.Adapters
{
    public interface IMethodInfoAdapter
    {
        IMethodInfoAdapter MakeGenericMethod(TypeAdapter type);
    }
}