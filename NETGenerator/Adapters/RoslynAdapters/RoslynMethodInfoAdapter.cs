using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynMethodInfoAdapter: RoslynMemberInfoAdapter, IMethodInfoAdapter
    {
        public ITypeAdapter ReturnType { get; protected set; }
        public ITypeAdapter DeclaringType { get; }
        public string Name { get; }
        public int MetadataToken { get; }
        public bool IsStatic { get; }
        public bool IsVirtual { get; }
        public bool IsAbstract { get; }
        public bool IsSpecialName { get; }
        public bool IsPublic { get; }
        public bool IsPrivate { get; }

        protected List<IParameterInfoAdapter> _parameters;
        protected List<IParameterBuilderAdapter> _parameterBuilders;

        public RoslynMethodInfoAdapter(string name, MethodAttributes attributes, ITypeAdapter returnType,
            ITypeAdapter[] parameterTypes)
        {
            Name = name;
            ReturnType = returnType;
            _parameterBuilders = new List<IParameterBuilderAdapter>();
            if(parameterTypes is object)
            {
                _parameters = new List<IParameterInfoAdapter>(parameterTypes.Select(paramType => new RoslynParameterInfoAdapter(paramType)));
            }
            
            IsPublic = (attributes & MethodAttributes.Public) != 0;
            IsStatic = (attributes & MethodAttributes.Static) != 0;
            IsVirtual = (attributes & MethodAttributes.Virtual) != 0;
            IsPrivate = (attributes & MethodAttributes.Private) != 0;
            IsAbstract = (attributes & MethodAttributes.Abstract) != 0;
            IsSpecialName = (attributes & MethodAttributes.SpecialName) != 0;
        }

        public IMethodInfoAdapter MakeGenericMethod(ITypeAdapter type)
        {
            throw new System.NotImplementedException();
        }

        public IMethodInfoAdapter MakeGenericMethod(ITypeAdapter[] types)
        {
            throw new System.NotImplementedException();
        }

        public object[] GetCustomAttributes(ITypeAdapter type, bool inherit)
        {
            throw new System.NotImplementedException();
        }

        public IMethodInfoAdapter GetGenericMethodDefinition()
        {
            throw new System.NotImplementedException();
        }

        public IParameterInfoAdapter[] GetParameters()
        {
            return _parameters.ToArray();
        }

        public void Invoke(object obj, object[] parameters)
        {
            throw new System.NotSupportedException();
        }
    }
}