using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynParameterBuilderAdapter: IParameterBuilderAdapter
    {
        public int Position { get; }
        public string Name { get; }
        public bool IsIn { get; }
        public bool IsOut { get; }
        public bool IsOptional { get; }

        public RoslynParameterBuilderAdapter(string name, ParameterAttributes attributes, int position)
        {
            Name = name;
            Position = position;
            if ((attributes & ParameterAttributes.In) != 0)
            {
                IsIn = true;
            }

            if ((attributes & ParameterAttributes.Out) != 0)
            {
                IsOut = true;
            }

            if ((attributes & ParameterAttributes.Optional) != 0)
            {
                IsOptional = true;
            }
        }
        
        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            throw new System.NotImplementedException();
        }

        public void SetCustomAttribute(IConstructorInfoAdapter constructor, byte[] binaryAttribute)
        {
            throw new System.NotImplementedException();
        }

        public void SetConstant(object value)
        {
            throw new System.NotImplementedException();
        }
    }
}