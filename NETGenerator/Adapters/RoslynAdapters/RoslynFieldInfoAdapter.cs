using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynFieldInfoAdapter: RoslynMemberInfoAdapter, IFieldInfoAdapter
    {
        public ITypeAdapter FieldType { get; }
        public bool IsLiteral { get; }
        public bool IsStatic { get; }
        public string Name { get; }
        public bool IsPublic { get; }
        public bool IsPrivate { get; }

        public RoslynFieldInfoAdapter(string name, ITypeAdapter type, FieldAttributes attributes)
        {
            FieldType = type;
            Name = name;
            IsLiteral = (attributes & FieldAttributes.Literal) != 0;
            IsStatic = (attributes & FieldAttributes.Static) != 0;
            IsPublic = (attributes & FieldAttributes.Public) != 0;
            IsPrivate = (attributes & FieldAttributes.Private) != 0;
        }

        public object GetRawConstantValue()
        {
            throw new System.NotImplementedException();
        }
    }
}