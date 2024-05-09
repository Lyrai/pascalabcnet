using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkGenericTypeParameterBuilderAdapter: FrameworkTypeAdapter, IGenericTypeParameterBuilderAdapter
    {
        public new GenericTypeParameterBuilder Adaptee { get; }

        public FrameworkGenericTypeParameterBuilderAdapter(GenericTypeParameterBuilder builder): base(builder)
        {
            Adaptee = builder;
        }
        
        private Type GetAdaptee(ITypeAdapter adapter)
        {
            return (adapter as FrameworkTypeAdapter).Adaptee;
        }

        private Type[] GetAdaptee(ITypeAdapter[] adapters)
        {
            return adapters.Select(t => (t as FrameworkTypeAdapter).Adaptee).ToArray();
        }

        public void SetInterfaceConstraints(ITypeAdapter[] interfaces)
        {
            Adaptee.SetInterfaceConstraints(GetAdaptee(interfaces));
        }

        public void SetGenericParameterAttributes(GenericParameterAttributes attributes)
        {
            Adaptee.SetGenericParameterAttributes(attributes);
        }

        public void SetBaseTypeConstraint(ITypeAdapter baseType)
        {
            Adaptee.SetBaseTypeConstraint(GetAdaptee(baseType));
        }
    }
}