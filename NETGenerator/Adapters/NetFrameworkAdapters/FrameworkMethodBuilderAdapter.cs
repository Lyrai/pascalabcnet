using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkMethodBuilderAdapter: FrameworkMethodInfoAdapter, IMethodBuilderAdapter
    {
        public new MethodBuilder Adaptee { get; }
        
        public FrameworkMethodBuilderAdapter(MethodBuilder builder): base(builder)
        {
            Adaptee = builder;
        }

        public IILGeneratorAdapter GetILGenerator()
        {
            return Adaptee.GetILGenerator().GetAdapter();
        }

        public void SetCustomAttribute(IConstructorInfoAdapter con, byte[] binaryAttribute)
        {
            Adaptee.SetCustomAttribute((con as FrameworkConstructorInfoAdapter).Adaptee, binaryAttribute);
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            Adaptee.SetCustomAttribute((attribute as FrameworkCustomAttributeBuilderAdapter).Adaptee);
        }

        public IParameterBuilderAdapter DefineParameter(int index, ParameterAttributes attributes, string name)
        {
            return Adaptee.DefineParameter(index, attributes, name).GetAdapter();
        }

        public void DefineGenericParameters(string[] names)
        {
            Adaptee.DefineGenericParameters(names);
        }

        public void SetReturnType(ITypeAdapter type)
        {
            Adaptee.SetReturnType((type as FrameworkTypeAdapter).Adaptee);
        }

        public void SetParameters(ITypeAdapter[] types)
        {
            var ts = types.Select(t => (t as FrameworkTypeAdapter).Adaptee).ToArray();
            Adaptee.SetParameters(ts);
        }

        public void SetMarshal(UnmanagedMarshal marshal)
        {
            Adaptee.SetMarshal(marshal);
        }

        public void SetImplementationFlags(MethodImplAttributes attributes)
        {
            Adaptee.SetImplementationFlags(attributes);
        }

        public ITypeAdapter[] GetGenericArguments()
        {
            return Adaptee.GetGenericArguments().Select(t => t.GetAdapter()).ToArray();
        }
    }
}