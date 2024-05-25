using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynConstructorInfoAdapter: RoslynMemberInfoAdapter, IConstructorInfoAdapter
    {
        public IParameterInfoAdapter[] Parameters => GetParameters();
        public ITypeAdapter DeclaringType { get; }
        public override bool IsPrivate { get; }
        public override bool IsAssembly { get; }
        public override bool IsFamily { get; }
        public override bool IsFamilyAndAssembly { get; }
        public override bool IsFamilyOrAssembly { get; }
        public override string Name => IsStatic ? ".cctor" : ".ctor";
        public override bool IsPublic { get; }
        public bool IsStatic { get; }
        public ITypeAdapter ReturnType => typeof(void).GetAdapter();
        public MethodAttributes Attributes { get; }

        protected List<IParameterInfoAdapter> _parameters;
        protected List<IParameterBuilderAdapter> _parameterBuilders;

        public RoslynConstructorInfoAdapter(MethodAttributes attributes, ITypeAdapter declaringType, ITypeAdapter[] parameters)
        {
            DeclaringType = declaringType;
            Attributes = attributes;
            _parameters = parameters is null
                ? new List<IParameterInfoAdapter>()
                : parameters.Select(type => new RoslynParameterInfoAdapter(type) as IParameterInfoAdapter).ToList();
            _parameterBuilders = new List<IParameterBuilderAdapter>();

            IsPublic = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
            IsPrivate = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private;
            IsFamily = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Family;
            IsAssembly = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Assembly;
            IsFamilyAndAssembly = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamANDAssem;
            IsFamilyOrAssembly = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamORAssem;

            if ((attributes & MethodAttributes.Static) != 0) IsStatic = true;
        }

        public virtual IParameterInfoAdapter[] GetParameters()
        {
            return _parameters.ToArray();
        }
        
        public ITypeAdapter[] GetGenericArguments()
        {
            throw new NotSupportedException();
        }

        public override IMemberInfoAdapter Instantiate(Dictionary<ITypeAdapter, ITypeAdapter> typeArguments,
            ITypeAdapter declaringType)
        {
            throw new NotSupportedException();
        }
    }
}