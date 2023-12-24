using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IGenericTypeParameterBuilderAdapter: ITypeAdapter
    {
        void SetInterfaceConstraints(ITypeAdapter[] interfaces);
        void SetGenericParameterAttributes(GenericParameterAttributes attributes);
        void SetBaseTypeConstraint(ITypeAdapter baseType);
    }
}