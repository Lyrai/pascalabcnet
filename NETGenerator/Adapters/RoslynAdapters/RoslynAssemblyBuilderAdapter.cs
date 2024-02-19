using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public class RoslynAssemblyBuilderAdapter: RoslynAssemblyAdapter, IAssemblyBuilderAdapter
    {
        private CSharpCompilation _compilation;
        private string _path;
        private PEModuleBuilder _moduleBuilder;
        
        public RoslynAssemblyBuilderAdapter(string assemblyName, string path)
        {
            _compilation = CSharpCompilation.Create(assemblyName).WithReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Console").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location)
            );

            _path = path;
        }
        
        public void DefineVersionInfoResource(string product, string productVersion, string company, string copyright,
            string trademark)
        {
            //throw new System.NotImplementedException();
        }

        public void DefineUnmanagedResource(string path)
        {
            //throw new System.NotImplementedException();
        }

        public void DefineUnmanagedResource(byte[] path)
        {
            //throw new System.NotImplementedException();
        }

        public IModuleBuilderAdapter DefineDynamicModule(string name, bool emitSymbolInfo)
        {
            _moduleBuilder = _compilation.CreateModuleBuilder(
                EmitOptions.Default,
                null,
                null,
                null,
                ImmutableArray<ResourceDescription>.Empty,
                null,
                DiagnosticBag.GetInstance(),
                CancellationToken.None
            ) as PEModuleBuilder;

            return _moduleBuilder;
        }

        public IModuleBuilderAdapter DefineDynamicModule(string name, string fileName, bool emitSymbolInfo)
        {
            return DefineDynamicModule(name, emitSymbolInfo);
        }

        public void SetCustomAttribute(IConstructorInfoAdapter con, byte[] binaryAttribute)
        {
            //throw new System.NotImplementedException();
        }

        public void SetEntryPoint(IMethodBuilderAdapter method, PEFileKinds fileKind)
        {
            OutputKind outputKind;
            switch (fileKind)
            {
                case PEFileKinds.ConsoleApplication:
                    outputKind = OutputKind.ConsoleApplication;
                    break;
                case PEFileKinds.Dll:
                    outputKind = OutputKind.DynamicallyLinkedLibrary;
                    break;
                case PEFileKinds.WindowApplication:
                    outputKind = OutputKind.WindowsApplication;
                    break;
                default:
                    outputKind = OutputKind.ConsoleApplication;
                    break;
            }
            _compilation = _compilation.WithOptions(new CSharpCompilationOptions(outputKind));
            _moduleBuilder.SetPEEntryPoint(method, DiagnosticBag.GetInstance());
        }

        public void SetCustomAttribute(ICustomAttributeBuilderAdapter attribute)
        {
            //throw new System.NotImplementedException();
        }

        public ITypeAdapter CreateInstance(string typeName)
        {
            throw new System.NotSupportedException();
        }

        public void Save(string filename)
        {
            Save(filename, PortableExecutableKinds.PE32Plus, ImageFileMachine.IA64);
        }

        public void Save(string filename, PortableExecutableKinds peKind, ImageFileMachine imageFile)
        {
            _moduleBuilder.CompilationFinished();
            var peStreamProvider = new Compilation.SimpleEmitStreamProvider(File.Open(filename, FileMode.Create));
            var success = _compilation.SerializeToPeStream(
                _moduleBuilder,
                peStreamProvider,
                null,
                null,
                null,
                null,
                DiagnosticBag.GetInstance(),
                EmitOptions.Default,
                null,
                CancellationToken.None
            );
            
            if (success)
            {
                GenerateRuntimeConfig(Path.GetFullPath(filename));
            }
        }
        
        private void GenerateRuntimeConfig(string path)
        {
            var configPath = Path.Combine(Directory.GetParent(path).FullName, Path.GetFileNameWithoutExtension(path) + ".runtimeconfig.json");
            using (var stream = new StreamWriter(configPath))
            {
                var netCoreVersion = "6.0.0";
                stream.WriteLine(
@"{
    ""runtimeOptions"": {
        ""tfm"": ""net6.0"",
        ""framework"": {
            ""name"": ""Microsoft.NETCore.App"",
            ""version"": """ + netCoreVersion + @"""
        }
    }
}");
            }
        }
    }
}