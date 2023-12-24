﻿namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IParameterBuilderAdapter
    {
        int Position { get; }
        
        void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute);
        void SetCustomAttribute(IConstructorInfoAdapter constructor, byte[] binaryAttribute);
        void SetConstant(object value);
    }
}