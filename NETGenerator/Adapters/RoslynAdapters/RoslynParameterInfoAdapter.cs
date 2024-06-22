using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynParameterInfoAdapter: IParameterInfoAdapter
    {
        public ITypeAdapter ParameterType { get; }
        public string Name { get; }
        public int Position { get; }
        public bool IsByRef { get; }

        public RoslynParameterInfoAdapter(ITypeAdapter type)
        {
            ParameterType = type;
        }

        public RoslynParameterInfoAdapter(RoslynParameterBuilderAdapter builder)
        {
            ParameterType = builder.ParameterType;
            Name = builder.Name ?? "";
            Position = builder.Position;
            IsByRef = builder.ParameterType.Name.EndsWith("&");
        }
    }
}