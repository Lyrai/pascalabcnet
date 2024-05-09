using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkParameterBuilderAdapter: IParameterBuilderAdapter
    {
        public int Position => Adaptee.Position;
        public string Name => Adaptee.Name;
        public ParameterBuilder Adaptee { get; }
        
        public FrameworkParameterBuilderAdapter(ParameterBuilder builder)
        {
            Adaptee = builder;
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            Adaptee.SetCustomAttribute((attribute as FrameworkCustomAttributeBuilderAdapter).Adaptee);
        }

        public void SetCustomAttribute(IConstructorInfoAdapter constructor, byte[] binaryAttribute)
        {
            Adaptee.SetCustomAttribute((constructor as FrameworkConstructorInfoAdapter).Adaptee, binaryAttribute);
        }

        public void SetConstant(object value)
        {
            Adaptee.SetConstant(value);
        }
        
        public override int GetHashCode()
        {
            return Adaptee.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FrameworkParameterBuilderAdapter builder))
            {
                return false;
            }

            return Adaptee.Equals(builder.Adaptee);
        }
    }
}