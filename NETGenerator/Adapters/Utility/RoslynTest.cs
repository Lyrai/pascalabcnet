using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NETGenerator.Adapters.Utility
{
    public static class RoslynTest
    {
        public static void Test()
        {
            var tree = SyntaxFactory.ParseCompilationUnit(@"
using System;
using System.Collections;
using System.Collections.Generic;

namespace Program
{
    public class Test<U> {
        private U[] arr;
        public U[] Arr1 => arr;
    }

    public class Program
    {
        public static void Main()
        {
            
        }
    }
}
");
            /*var ass = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("test123"), AssemblyBuilderAccess.Run);
            var module = ass.DefineDynamicModule("mod1");
            var type = module.DefineType("TestType");
            var param = type.DefineGenericParameters("T").Cast<Type>().ToArray();
            var meth1 = type.DefineMethod("Test1", MethodAttributes.Public, typeof(void), param);
            //var ctor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[0]);
            var instance = type.MakeGenericType(typeof(int));
            var meth2 = type.DefineMethod("Test2", MethodAttributes.Public, typeof(void), param);*/
            var compilation = CSharpCompilation.Create("test1111", new List<SyntaxTree>() {tree.SyntaxTree}, new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Console").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Collections").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime.Numerics").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Linq").Location)
            },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication, optimizationLevel: OptimizationLevel.Debug, concurrentBuild: false));
            var result = compilation.Emit("aa2.exe");
            var assembly = compilation.Assembly;
            /*var meth =
                ((assembly.GlobalNamespace.GetMembers("Program")[0] as SourceNamespaceSymbol).GetMembers("Program")[0] as SourceNamedTypeSymbol).GetMembers("RefTest")[0] as SourceMemberMethodSymbol;

            var str = meth.ToString();
            Console.WriteLine(meth);*/
        }
    }
}