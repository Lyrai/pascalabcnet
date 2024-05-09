using System;
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
        public ITypeAdapter ParameterType { get; }
        public ParameterAttributes Attributes { get; }

        public RoslynParameterBuilderAdapter(string name, ParameterAttributes attributes, int position, ITypeAdapter type)
        {
            Name = name;
            Position = position;
            ParameterType = type;
            Attributes = attributes;
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
            Console.WriteLine("RoslynParameterBuilderAdapter.SetCustomAttribute not implemented");
            //throw new System.NotImplementedException();
        }

        public void SetConstant(object value)
        {
            Console.WriteLine("RoslynParameterBuilderAdapter.SetConstant not implemented");
            //throw new System.NotImplementedException();
        }

        public IParameterInfoAdapter GetInfo()
        {
            return new RoslynParameterInfoAdapter(this);
        }

        public IParameterBuilderAdapter WithTypeSubstituted(ITypeAdapter type)
        {
            return new RoslynParameterBuilderAdapter(Name, Attributes, Position, type);
        }
    }
}