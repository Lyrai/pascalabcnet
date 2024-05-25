using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynPropertyBuilderAdapter: RoslynPropertyInfoAdapter, IPropertyBuilderAdapter
    {
        public RoslynPropertyBuilderAdapter(string name, PropertyAttributes attributes) : base(name, attributes)
        { }
        
        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            throw new System.NotImplementedException();
        }

        public void SetGetMethod(IMethodBuilderAdapter method)
        {
            _getMethod = method;
            PropertyType = _getMethod.ReturnType;
        }

        public void SetSetMethod(IMethodBuilderAdapter method)
        {
            _setMethod = method;
            if (PropertyType is object && !Equals(PropertyType, method.GetParameters()[0].ParameterType))
            {
                throw new InvalidOperationException("Property getter/setter type mismatch");
            }

            PropertyType = method.GetParameters()[0].ParameterType;
        }

        public override IMemberInfoAdapter Instantiate(Dictionary<ITypeAdapter, ITypeAdapter> typeArguments,
            ITypeAdapter declaringType)
        {
            throw new NotImplementedException();
        }
    }
}