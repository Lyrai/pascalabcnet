using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Emit;
using NETGenerator.Adapters.Utility;
using BindingDiagnosticBag = Microsoft.CodeAnalysis.CSharp.BindingDiagnosticBag;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynAssemblyBuilderAdapter: RoslynAssemblyAdapter, IAssemblyBuilderAdapter
    {
        private CSharpCompilation _compilation;
        private string _path;
        private PEModuleBuilder _moduleBuilderRaw;
        private IMethodBuilderAdapter _entryPoint = null;
        
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
            throw new System.NotImplementedException();
        }

        public void DefineUnmanagedResource(string path)
        {
            throw new System.NotImplementedException();
        }

        public void DefineUnmanagedResource(byte[] path)
        {
            throw new System.NotImplementedException();
        }

        public IModuleBuilderAdapter DefineDynamicModule(string name, bool emitSymbolInfo)
        {
            _moduleBuilder = new RoslynModuleBuilderAdapter(this);

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
            _entryPoint = method;
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
            Save(filename, PortableExecutableKinds.PE32Plus, ImageFileMachine.I386);
        }

        public void Save(string filename, PortableExecutableKinds peKind, ImageFileMachine imageFile)
        {
            var moduleBuilder = CreateModuleBuilder();
            CreateSymbols();
            RealizeMethods();
            moduleBuilder.CompilationFinished();
            var peStreamProvider = new Compilation.SimpleEmitStreamProvider(File.Open(filename, FileMode.Create));
            var success = _compilation.SerializeToPeStream(
                moduleBuilder,
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

        private PEModuleBuilder CreateModuleBuilder()
        {
            return _moduleBuilderRaw = _compilation.CreateModuleBuilder(
                EmitOptions.Default,
                null,
                null,
                null,
                ImmutableArray<ResourceDescription>.Empty,
                null,
                DiagnosticBag.GetInstance(),
                CancellationToken.None
            ) as PEModuleBuilder;
        }

        private void CreateSymbols()
        {
            var globalNamespaceMembers = CreateDeclarations();
            var globalNamespaceSymbol = CreateNamespaceSymbol("", globalNamespaceMembers);
            (_compilation.SourceModule as SourceModuleSymbol).SetGlobalNamespace(globalNamespaceSymbol);

            foreach (var type in GetTypes())
            {
                var typeSymbol = globalNamespaceSymbol.GetType(type);
                var baseType = _compilation.Assembly.GetType(type.BaseType);
                typeSymbol.SetBaseType(baseType);
                
                foreach (var method in type.GetMethods())
                {
                    var symbol = CreateMethodSymbol(method, typeSymbol);
                }
            }
        }

        private PascalMethodSymbol CreateMethodSymbol(IMethodInfoAdapter method, NamedTypeSymbol declaringType)
        {
            var returnType = _compilation.Assembly.GetType(method.ReturnType);
            var symbol = new PascalMethodSymbol(declaringType, false, returnType);
            int paramCnt = 0;
            var parameters = new List<ParameterSymbol>();
            foreach (var param in method.GetParameters())
            {
                var type = _compilation.Assembly.GetType(param.ParameterType);
                var paramSymbol = new PascalParameterSymbol(symbol, paramCnt, type);
                parameters.Add(paramSymbol);
                ++paramCnt;
            }

            symbol.SetParameters(parameters.ToImmutableArray());

            return symbol;
        }

        private ImmutableArray<SingleNamespaceOrTypeDeclaration> CreateDeclarations()
        {
            var types = GetTypes().Cast<RoslynTypeBuilderAdapter>();
            return DeclarationsUtility.CreateDeclarations(types);
        }
        
        private SourceNamespaceSymbol CreateNamespaceSymbol(string name, ImmutableArray<SingleNamespaceOrTypeDeclaration> members)
        {
            var namespaceDeclaration = DeclarationsUtility.CreateNamespaceDeclaration(name, members);
            var mergedNamespace = MergedNamespaceDeclaration.Create(namespaceDeclaration);
            
            return new SourceNamespaceSymbol(
                _compilation.SourceModule as SourceModuleSymbol,
                _compilation.SourceModule,
                mergedNamespace, 
                BindingDiagnosticBag.GetInstance()
            );
        }

        private void RealizeMethods()
        {
            
        }
    }
}