using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public abstract class GenericTypeParameterBuilderAdapter: TypeAdapter
    {
        public abstract void SetInterfaceConstraints(TypeAdapter[] interfaces);
        public abstract void SetGenericParameterAttributes(GenericParameterAttributes attributes);
        public abstract void SetBaseTypeConstraint(TypeAdapter baseType);
    }
}