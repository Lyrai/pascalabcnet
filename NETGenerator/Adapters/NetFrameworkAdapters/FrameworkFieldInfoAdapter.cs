using System;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkFieldInfoAdapter: FrameworkMemberInfoAdapter, IFieldInfoAdapter
    {
        public ITypeAdapter FieldType => Adaptee.FieldType.GetAdapter();
        public bool IsLiteral => Adaptee.IsLiteral;
        public bool IsStatic => Adaptee.IsStatic;
        public override string Name => Adaptee.Name;
        public override bool IsPrivate => Adaptee.IsPrivate;
        public override bool IsAssembly => Adaptee.IsAssembly;
        public override bool IsFamily => Adaptee.IsFamily;
        public override bool IsPublic => Adaptee.IsPublic;
        public override bool IsFamilyAndAssembly => Adaptee.IsFamilyAndAssembly;
        public override bool IsFamilyOrAssembly => Adaptee.IsFamilyOrAssembly;
        public ITypeAdapter DeclaringType => Adaptee.DeclaringType.GetAdapter();
        public FieldAttributes Attributes => Adaptee.Attributes;
        public new FieldInfo Adaptee { get; }

        public FrameworkFieldInfoAdapter(FieldInfo info): base(info)
        {
            Adaptee = info;
        }
        
        public object GetRawConstantValue()
        {
            return Adaptee.GetRawConstantValue();
        }
    }
}