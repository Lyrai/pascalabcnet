﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynFieldInfoAdapter: RoslynMemberInfoAdapter, IFieldInfoAdapter
    {
        public ITypeAdapter FieldType { get; }
        public bool IsLiteral { get; }
        public bool IsStatic { get; }
        public override string Name { get; }
        public ITypeAdapter DeclaringType { get; }
        public override bool IsPublic { get; }
        public override bool IsPrivate { get; }
        public FieldAttributes Attributes { get; }

        public RoslynFieldInfoAdapter(ITypeAdapter declaringType, string name, ITypeAdapter type, FieldAttributes attributes)
        {
            FieldType = type;
            Name = name;
            IsLiteral = (attributes & FieldAttributes.Literal) != 0;
            IsStatic = (attributes & FieldAttributes.Static) != 0;
            IsPublic = (attributes & FieldAttributes.Public) != 0;
            IsPrivate = (attributes & FieldAttributes.Private) != 0;
            DeclaringType = declaringType;
            Attributes = attributes;
        }

        public object GetRawConstantValue()
        {
            throw new System.NotImplementedException();
        }

        public override IMemberInfoAdapter Instantiate(Dictionary<ITypeAdapter, ITypeAdapter> typeArguments,
            ITypeAdapter declaringType)
        {
            throw new NotSupportedException();
        }
    }
}