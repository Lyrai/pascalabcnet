using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynMethodInfoAdapter: RoslynMemberInfoAdapter, IMethodInfoAdapter
    {
        public ITypeAdapter ReturnType { get; protected set; }
        public ITypeAdapter DeclaringType { get; protected set; }
        public override string Name { get; }
        public int MetadataToken { get; }
        public bool IsStatic { get; }
        public bool IsVirtual { get; }
        public bool IsAbstract { get; }
        public bool IsSpecialName { get; }
        public override bool IsPublic { get; }
        public override bool IsPrivate { get; }
        public override bool IsAssembly { get; }
        public override bool IsFamily { get; }
        public override bool IsFamilyAndAssembly { get; }
        public override bool IsFamilyOrAssembly { get; }
        public bool IsGenericMethod { get; protected set; }
        public bool IsGenericMethodDefinition { get; protected set; }
        public MethodAttributes Attributes { get; }
        
        protected List<RefKind> _parameterRefKinds;

        public RoslynMethodInfoAdapter(string name, MethodAttributes attributes, ITypeAdapter returnType,
            ITypeAdapter[] parameterTypes)
        {
            Name = name;
            ReturnType = returnType ?? typeof(void).GetAdapter();
            Attributes = attributes;
            
            IsPublic = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
            IsStatic = (attributes & MethodAttributes.Static) != 0;
            IsVirtual = (attributes & MethodAttributes.Virtual) != 0;
            IsPrivate = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private;
            IsAbstract = (attributes & MethodAttributes.Abstract) != 0;
            IsSpecialName = (attributes & MethodAttributes.SpecialName) != 0;
            IsFamily = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Family;
            IsAssembly = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Assembly;
            IsFamilyAndAssembly = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamANDAssem;
            IsFamilyOrAssembly = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamORAssem;
            IsGenericMethod = false;
        }

        public virtual IMethodInfoAdapter MakeGenericMethod(params ITypeAdapter[] types)
        {
            throw new NotSupportedException();
        }

        public object[] GetCustomAttributes(ITypeAdapter type, bool inherit)
        {
            throw new System.NotImplementedException();
        }

        public IMethodInfoAdapter GetGenericMethodDefinition()
        {
            throw new System.NotImplementedException();
        }

        public virtual IParameterInfoAdapter[] GetParameters()
        {
            throw new NotSupportedException();
        }

        public override IMemberInfoAdapter Instantiate(Dictionary<ITypeAdapter, ITypeAdapter> typeArguments,
            ITypeAdapter declaringType)
        {
            throw new NotSupportedException();
        }

        public void Invoke(object obj, object[] parameters)
        {
            throw new System.NotSupportedException();
        }

        public virtual ITypeAdapter[] GetGenericArguments()
        {
            throw new NotSupportedException();
        }
    }
}