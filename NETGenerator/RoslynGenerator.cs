using PascalABCCompiler.NETGenerator;
using PascalABCCompiler.NetHelper;
using PascalABCCompiler.SemanticTree;

namespace NETGenerator
{
    public class RoslynGenerator: IILConverter
    {
        public void ConvertFromTree(IProgramNode p, string TargetFileName, string SourceFileName, CompilerOptions options,
            string[] ResourceFiles)
        {
            throw new System.NotImplementedException();
        }

        public void EmitAssemblyRedirects(AssemblyResolveScope resolveScope, string targetAssemblyPath)
        {
            throw new System.NotImplementedException();
        }
    }
}