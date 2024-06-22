﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Emit;
using NETGenerator.Adapters.Utility;
using Roslyn.Utilities;
using BindingDiagnosticBag = Microsoft.CodeAnalysis.CSharp.BindingDiagnosticBag;
using MethodBody = Microsoft.CodeAnalysis.CodeGen.MethodBody;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynAssemblyBuilderAdapter: RoslynAssemblyAdapter, IAssemblyBuilderAdapter
    {
        public CSharpCompilation Compilation => _compilation;
        
        private CSharpCompilation _compilation;
        private string _path;
        private PEModuleBuilder _moduleBuilderRaw;
        private IMethodBuilderAdapter _entryPoint = null;
        private MethodSymbol _entryPointSymbol;
        
        public RoslynAssemblyBuilderAdapter(string assemblyName, string path)
        {
            _compilation = CreateCompilation(assemblyName);
            _path = path;
            InitModules();
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
            
            moduleBuilder.SetPEEntryPoint(_entryPointSymbol, DiagnosticBag.GetInstance());
            moduleBuilder.CompilationFinished();

            var resultFilePath = Path.Combine(_path, filename);
            var peStreamProvider = new Compilation.SimpleEmitStreamProvider(File.Open(resultFilePath, FileMode.Create));
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
                GenerateRuntimeConfig(Path.GetFullPath(resultFilePath));
            }
        }
        
        private void GenerateRuntimeConfig(string path)
        {
            var configPath = Path.Combine(Directory.GetParent(path).FullName, Path.GetFileNameWithoutExtension(path) + ".runtimeconfig.json");
            using (var stream = new StreamWriter(configPath))
            {
                var netCoreVersion = Environment.Version;
                stream.WriteLine(
@"{
    ""runtimeOptions"": {
        ""tfm"": ""net" +netCoreVersion.Major + "." + netCoreVersion.Minor + @""",
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
            _moduleBuilderRaw = _compilation.CreateModuleBuilder(
                EmitOptions.Default,
                null,
                null,
                null,
                ImmutableArray<ResourceDescription>.Empty,
                null,
                DiagnosticBag.GetInstance(),
                CancellationToken.None
            ) as PEModuleBuilder;
            ResolveHelper.Init(_compilation.Assembly);

            return _moduleBuilderRaw;
        }

        private void CreateSymbols()
        {
            var globalNamespaceMembers = CreateDeclarations();
            var globalNamespaceSymbol = CreateNamespaceSymbol("", globalNamespaceMembers);
            (_compilation.SourceModule as SourceModuleSymbol).SetGlobalNamespace(globalNamespaceSymbol);
            
            CreateTypes(globalNamespaceSymbol);
        }

        private void CreateTypes(NamespaceSymbol ns)
        {
            var nsMembers = ns.GetMembersUnordered();
            foreach (var nested in nsMembers.OfType<NamespaceSymbol>())
            {
                CreateTypes(nested);
            }
            
            foreach (var type in GetTypes().Where(t => t.Namespace == ns.QualifiedName))
            {
                CreateTypeSymbol(type, ns);
            }
            
            foreach (var symbol in ns.GetMembersUnordered().OfType<SourceNamedTypeSymbol>())
            {
                symbol.ResetMembersAndInitializers();
                symbol.GetMembersAndInitializers();
            }
        }

        private TypeSymbol CreateTypeSymbol(ITypeAdapter type, NamespaceSymbol ns, SourceNamedTypeSymbol sym = null)
        {
            var typeSymbol = ResolveHelper.ResolveType(type) as SourceNamedTypeSymbol;
            SymbolTable.Set(type, typeSymbol);
            if(type.IsGenericType)
            {
                var types = new List<TypeParameterSymbol>();
                foreach (var genericArgument in type.GetGenericArguments())
                {
                    var symbol =
                        new PascalTypeParameterSymbol(genericArgument as RoslynGenericTypeParameterBuilderAdapter);
                    symbol.SetContainingSymbol(typeSymbol);
                    types.Add(symbol);
                }

                typeSymbol.SetTypeParameters(types.ToImmutableArray());
            }
            
            typeSymbol.SetInterfaces(type.ImplementedInterfaces.Select(ResolveHelper.ResolveNamedType).ToImmutableArray());
            
            if(!type.IsInterface)
            {
                var baseType = ResolveHelper.ResolveNamedType(type.BaseType);
                typeSymbol.SetBaseType(baseType);
            }

            var members = new Dictionary<string, List<Symbol>>();
            
            foreach (var method in type.GetMethods())
            {
                var symbol = CreateMethodSymbol(method, typeSymbol);
                SymbolTable.Set(method, symbol);
                if (members.TryGetValue(method.Name, out var methods))
                {
                    methods.Add(symbol);
                }
                else
                {
                    members[method.Name] = new List<Symbol> { symbol };
                }
            }
            
            foreach (var ctor in type.GetConstructors())
            {
                var symbol = CreateConstructorSymbol(ctor, typeSymbol);
                SymbolTable.Set(ctor, symbol);
                if (members.TryGetValue(ctor.Name, out var ctors))
                {
                    ctors.Add(symbol);
                }
                else
                {
                    members[ctor.Name] = new List<Symbol> { symbol };
                }
            }

            foreach (var field in type.GetFields())
            {
                var symbol = CreateFieldSymbol(field, typeSymbol);
                SymbolTable.Set(field, symbol);
                if (members.TryGetValue(field.Name, out var methods))
                {
                    methods.Add(symbol);
                }
                else
                {
                    members[field.Name] = new List<Symbol> { symbol };
                }
            }

            foreach (var property in type.GetProperties())
            {
                var getMethod = property.GetGetMethod() is null ? null : members[property.GetGetMethod().Name][0];
                var setMethod = property.GetSetMethod() is null ? null : members[property.GetSetMethod().Name][0];
                
                var symbol = CreatePropertySymbol(property, typeSymbol, getMethod, setMethod);
                SymbolTable.Set(property, symbol);
                if (members.TryGetValue(property.Name, out var properties))
                {
                    properties.Add(symbol);
                }
                else
                {
                    members[property.Name] = new List<Symbol> { symbol };
                }
            }
            
            foreach (var nested in type.GetNestedTypes())
            {
                //var t = typeSymbol.GetMembers(nested.Name).OfType<SourceNamedTypeSymbol>().First();
                var symbol = CreateTypeSymbol(nested, ns) as SourceNamedTypeSymbol;
                symbol.ResetMembersAndInitializers();
                symbol.GetMembersAndInitializers();
                if (members.TryGetValue(symbol.Name, out var types))
                {
                    types.Add(symbol);
                }
                else
                {
                    members[symbol.Name] = new List<Symbol> { symbol };
                }
            }

            var membersDict = new Dictionary<string, ImmutableArray<Symbol>>();
            foreach (var (key, value) in members)
            {
                membersDict.Add(key, value.ToImmutableArray());
            }
            
            typeSymbol.SetMembersDictionary(membersDict);

            return typeSymbol;
        }

        private PascalPropertySymbol CreatePropertySymbol(IPropertyInfoAdapter property, NamedTypeSymbol declaringType, Symbol getMethod, Symbol setMethod)
        {
            return new PascalPropertySymbol(property, declaringType, getMethod, setMethod);
        }

        private PascalMethodSymbol CreateMethodSymbol(IMethodInfoAdapter method, NamedTypeSymbol declaringType)
        {
            return new PascalMethodSymbol(method, declaringType, false);
        }
        
        private PascalConstructorSymbol CreateConstructorSymbol(IConstructorInfoAdapter method, NamedTypeSymbol declaringType)
        {
            return new PascalConstructorSymbol(method, declaringType);
        }
        
        private PascalFieldSymbol CreateFieldSymbol(IFieldInfoAdapter field, SourceNamedTypeSymbol containingType)
        {
            var type = _compilation.Assembly.GetType(field.FieldType);
            return new PascalFieldSymbol(field, containingType, field.Name, type);
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
            var bindingDiagnostics = new BindingDiagnosticBag(DiagnosticBag.GetInstance());
            var embeddedTextsCompiler = new MethodCompiler(
                _compilation,
                _moduleBuilderRaw,
                false,
                false,
                true,
                bindingDiagnostics,
                null,
                null,
                CancellationToken.None);
            embeddedTextsCompiler.CompileSynthesizedMethods(_moduleBuilderRaw.GetEmbeddedTypes(bindingDiagnostics), bindingDiagnostics);
            
            RealizeMethods((_compilation.SourceModule as SourceModuleSymbol).GlobalNamespace);
        }
        
        private void RealizeMethods(NamespaceSymbol ns)
        {
            var members = ns.GetMembersUnordered();
            foreach (var nested in members.OfType<NamespaceSymbol>())
            {
                RealizeMethods(nested);
            }

            foreach (var type in GetTypes().Where(t => t.Namespace == ns.QualifiedName && !t.IsInterface))
            {
                var typeSymbol = ns.GetMembers(type.Name).First() as NamedTypeSymbol;

                foreach (var method in type.GetMethods().Concat<IMethodBaseAdapter>(type.GetConstructors()))
                {
                    var generator = (method as IMethodBuilderAdapter)?.GetILGenerator() ?? (method as IConstructorBuilderAdapter).GetILGenerator();
                    var builder = generator as RoslynILGeneratorAdapter;
                    //var methodSymbol = ResolveHelper.ResolveMethodInType(typeSymbol, method);
                    var methodSymbol = SymbolTable.Get(method) as MethodSymbol;
                    var realizedBuilder = builder.Realize(_moduleBuilderRaw, methodSymbol, OptimizationLevel.Release, Equals(method.ReturnType, typeof(void).GetAdapter()));
                    var methodBody = new MethodBody(
                        realizedBuilder.RealizedIL,
                        realizedBuilder.MaxStack,
                        methodSymbol.GetCciAdapter(),
                        new DebugId(0, _moduleBuilderRaw.CurrentGenerationOrdinal),
                        realizedBuilder.LocalSlotManager.LocalsInOrder(),
                        realizedBuilder.RealizedSequencePoints,
                        null,
                        realizedBuilder.RealizedExceptionHandlers,
                        realizedBuilder.AreLocalsZeroed,
                        false,
                        realizedBuilder.GetAllScopes(),
                        realizedBuilder.HasDynamicLocal,
                        null,
                        ImmutableArray<LambdaDebugInfo>.Empty,
                        ImmutableArray<ClosureDebugInfo>.Empty,
                        null,
                        default,
                        default,
                        default,
                        StateMachineStatesDebugInfo.Create(null, ImmutableArray<StateMachineStateDebugInfo>.Empty),
                        null,
                        ImmutableArray<SourceSpan>.Empty,
                        methodSymbol is SynthesizedPrimaryConstructor
                    );
                    _moduleBuilderRaw.SetMethodBody(methodSymbol, methodBody);

                    if (method.Equals(_entryPoint))
                    {
                        _entryPointSymbol = methodSymbol;
                    }
                }
            }
        }

        private CSharpCompilation CreateCompilation(string assemblyName)
        {
            return CSharpCompilation
                .Create(assemblyName)
                .WithReferences(CreateReferences());
        }

        private IEnumerable<MetadataReference> CreateReferences()
        {
            var coreLibLocation = new DirectoryInfo(typeof(object).Assembly.Location);
            var version = coreLibLocation.Parent.Name;
            var searchBase = coreLibLocation.Parent.Parent.Parent.FullName;
            var searchLocations = new string[] {"Microsoft.NETCore.App", "Microsoft.WindowsDesktop.App"};
            var loaded = new List<MetadataReference>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.Location != "" && !assembly.Location.Contains("GAC_MSIL"))
                {
                    loaded.Add(MetadataReference.CreateFromFile(assembly.Location));
                }

                try
                {
                    var tmp = Assembly.Load(assembly.GetName().Name);
                    if (tmp.Location != "")
                    {
                        loaded.Add(MetadataReference.CreateFromFile(tmp.Location));
                    }
                } catch(IOException e) {}

                foreach(var location in searchLocations)
                {
                    var candidate = Path.Combine(searchBase, location, version, assembly.GetName().Name + ".dll");
                    if (File.Exists(candidate))
                    {
                        loaded.Add(MetadataReference.CreateFromFile(candidate));
                    }
                }
            }

            return loaded;
        }

        private void InitModules()
        {
            ResolveHelper.Init(_compilation.Assembly);
            SymbolTable.Init();
            AdapterExtensions.Init();
        }
    }
}