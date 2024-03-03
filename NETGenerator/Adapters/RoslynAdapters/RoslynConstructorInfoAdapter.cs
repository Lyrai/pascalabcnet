using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynConstructorInfoAdapter: RoslynMemberInfoAdapter, IConstructorInfoAdapter
    {
        public IParameterInfoAdapter[] Parameters => GetParameters();
        public ITypeAdapter DeclaringType { get; }

        protected List<IParameterInfoAdapter> _parameters;
        protected List<IParameterBuilderAdapter> _parameterBuilders;

        public RoslynConstructorInfoAdapter(MethodAttributes attributes, ITypeAdapter declaringType, ITypeAdapter[] parameters)
        {
            DeclaringType = declaringType;
            _parameters = parameters is null
                ? new List<IParameterInfoAdapter>()
                : parameters.Select(type => new RoslynParameterInfoAdapter(type) as IParameterInfoAdapter).ToList();
            _parameterBuilders = new List<IParameterBuilderAdapter>();
        }

        public IParameterInfoAdapter[] GetParameters()
        {
            return _parameters.ToArray();
        }
    }
}