using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using Microsoft.Cci;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Roslyn.Utilities;
using System.Linq;
using System.Reflection;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public partial class RoslynILGeneratorAdapter
    {
        private class ILConverter
        {
            private List<Instruction> _instructions;
            private List<object> _arguments;
            private Dictionary<int, object> _labels;
            private Dictionary<ILocalBuilderAdapter, LocalDefinition> _localsByBuilder;
            private Dictionary<int, LocalDefinition> _localsByIndex;
            private PEModuleBuilder _moduleBuilder;

            public ILConverter(List<Instruction> instructions, List<object> arguments)
            {
                _instructions = instructions;
                _arguments = arguments;
                _labels = new Dictionary<int, object>();
                _localsByBuilder = new Dictionary<ILocalBuilderAdapter, LocalDefinition>();
                _localsByIndex = new Dictionary<int, LocalDefinition>();
            }

            public ILBuilder Realize(PEModuleBuilder moduleBuilder, OptimizationLevel optimizationLevel)
            {
                _moduleBuilder = moduleBuilder;
                ILBuilder builder = new ILBuilder(moduleBuilder, new LocalSlotManager(null), optimizationLevel, true);
                var argument = _arguments.GetEnumerator();
                object currentExceptionLabel = null;
                
                foreach (var instruction in _instructions)
                {
                    switch (instruction)
                    {
                        case Instruction.Emit:
                            var emitArg = argument.Current as EmitInstruction;
                            argument.MoveNext();
                            var opcode = GetOpCode(emitArg.OpCode);
                            if (emitArg.Argument is null)
                            {
                                builder.EmitOpCode(opcode);
                                break;
                            }

                            switch (emitArg.Argument)
                            {
                                case string s:
                                    builder.EmitStringConstant(s);
                                    break;
                                case float f:
                                    builder.EmitSingleConstant(f);
                                    break;
                                case double d:
                                    builder.EmitDoubleConstant(d);
                                    break;
                                case long l:
                                    builder.EmitLongConstant(l);
                                    break;
                                case sbyte sb:
                                    builder.EmitSByteConstant(sb);
                                    break;
                                case byte b:
                                case int i:
                                    EmitNumericArgument(builder, opcode, emitArg.Argument);
                                    break;
                                case IFieldInfoAdapter field:
                                    EmitField(builder, opcode, field);
                                    break;
                                case IMethodInfoAdapter method:
                                    EmitMethod(
                                        builder,
                                        opcode,
                                        method.DeclaringType,
                                        method.Name,
                                        method.GetParameters().Select(p => p.ParameterType).ToArray()
                                    );
                                    break;
                                case IConstructorInfoAdapter constructor:
                                    EmitMethod(
                                        builder,
                                        opcode,
                                        constructor.DeclaringType,
                                        ".ctor",
                                        constructor.GetParameters().Select(p => p.ParameterType).ToArray()
                                    );
                                    break;
                                case ILocalBuilderAdapter local:
                                    EmitLocal(builder, opcode, local);
                                    break;
                                case ITypeAdapter type:
                                    EmitType(builder, opcode, type);
                                    break;
                                case Label label:
                                    builder.EmitBranch(opcode, _labels[GetLabelNum(label)]);
                                    break;
                                case Label[] labels:
                                    var objectLabels = labels.Select(l => GetLabelNum(l)).Select(l => _labels[l]).ToArray();
                                    builder.EmitSwitch(objectLabels);
                                    break;
                                default:
                                    throw ExceptionUtilities.Unreachable();
                            }
                            break;
                        case Instruction.DeclareLocal:
                            var localArg = argument.Current as RoslynLocalBuilderAdapter;
                            argument.MoveNext();
                            var definition = builder.LocalSlotManager.AllocateSlot(GetToken(ResolveType(localArg.LocalType)), LocalSlotConstraints.None);
                            _localsByBuilder.Add(localArg, definition);
                            _localsByIndex.Add(definition.SlotIndex, definition);
                            break;
                        case Instruction.EmitCall:
                            var emitCallArgument = argument.Current as EmitCallInstruction;
                            argument.MoveNext();
                            var m = emitCallArgument.Method;
                            EmitMethod(
                                builder, 
                                GetOpCode(emitCallArgument.OpCode), 
                                m.DeclaringType, 
                                m.Name, 
                                m.GetParameters().Select(p => p.ParameterType).ToArray()
                            );
                            break;
                        case Instruction.MarkSequencePoint:
                            break;
                        case Instruction.DefineLabel:
                            var labelArgument = (Label) argument.Current;
                            argument.MoveNext();
                            _labels.Add(GetLabelNum(labelArgument), new object());
                            break;
                        case Instruction.MarkLabel:
                            var labelArgument1 = (Label) argument.Current;
                            argument.MoveNext();
                            builder.MarkLabel(_labels[GetLabelNum(labelArgument1)]);
                            break;
                        case Instruction.BeginExceptionBlock:
                            var tryLabel = (Label) argument.Current;
                            var tryLabelObject = new object();
                            _labels.Add(GetLabelNum(tryLabel), tryLabelObject);
                            currentExceptionLabel = tryLabelObject;
                            argument.MoveNext();
                            builder.OpenLocalScope(ScopeType.TryCatchFinally);
                            builder.OpenLocalScope(ScopeType.Try);
                            break;
                        case Instruction.BeginCatchBlock:
                            var exceptionType = argument.Current as ITypeAdapter;
                            argument.MoveNext();
                            builder.CloseLocalScope();
                            builder.OpenLocalScope(ScopeType.Catch, GetToken(ResolveType(exceptionType)));
                            break;
                        case Instruction.BeginFinallyBlock:
                            builder.CloseLocalScope();
                            builder.OpenLocalScope(ScopeType.Finally);
                            break;
                        case Instruction.EndExceptionBlock:
                            builder.CloseLocalScope();
                            builder.CloseLocalScope();
                            builder.MarkLabel(currentExceptionLabel);
                            break;
                    }
                }
                
                builder.Realize();
                builder.FreeBasicBlocks();
                return builder;
            }

            private ILOpCode GetOpCode(OpCode opcode)
            {
                return (ILOpCode) opcode.Value;
            }

            private int GetLabelNum(Label label)
            {
                return (int)label
                    .GetType()
                    .GetField("m_label", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(label);
            }

            private void EmitNumericArgument(ILBuilder builder, ILOpCode opcode, object argument)
            {
                Debug.Assert(argument is int || argument is byte);
                var arg = (int) argument;
                switch (opcode)
                {
                    case ILOpCode.Ldc_i4:
                        builder.EmitIntConstant(arg);
                        break;
                    case ILOpCode.Ldc_i8:
                        builder.EmitLongConstant(arg);
                        break;
                    case ILOpCode.Ldarg:
                    case ILOpCode.Ldarg_s:
                        builder.EmitLoadArgumentOpcode(arg);
                        break;
                    case ILOpCode.Ldarga:
                    case ILOpCode.Ldarga_s:
                        builder.EmitLoadArgumentAddrOpcode(arg);
                        break;
                    case ILOpCode.Ldloc:
                    case ILOpCode.Ldloc_s:
                        builder.EmitLocalLoad(GetLocalDefinition(arg));
                        break;
                    case ILOpCode.Ldloca:
                    case ILOpCode.Ldloca_s:
                        builder.EmitLocalAddress(GetLocalDefinition(arg));
                        break;
                    case ILOpCode.Starg:
                    case ILOpCode.Starg_s:
                        builder.EmitStoreArgumentOpcode(arg);
                        break;
                    case ILOpCode.Stloc:
                    case ILOpCode.Stloc_s:
                        builder.EmitLocalStore(GetLocalDefinition(arg));
                        break;
                    default:
                        throw ExceptionUtilities.Unreachable();
                }
            }

            private void EmitLocal(ILBuilder builder, ILOpCode opcode, ILocalBuilderAdapter localBuilder)
            {
                var definition = GetLocalDefinition(localBuilder);
                switch (opcode)
                {
                    case ILOpCode.Ldloc:
                    case ILOpCode.Ldloc_s:
                        builder.EmitLocalLoad(definition);
                        break;
                    case ILOpCode.Ldloca:
                    case ILOpCode.Ldloca_s:
                        builder.EmitLocalAddress(definition);
                        break;
                    case ILOpCode.Stloc:
                    case ILOpCode.Stloc_s:
                        builder.EmitLocalStore(definition);
                        break;
                    case ILOpCode.Stloc_0:
                        throw ExceptionUtilities.UnexpectedValue(opcode);
                    default:
                        throw ExceptionUtilities.Unreachable();
                }
            }

            private void EmitField(ILBuilder builder, ILOpCode opcode, IFieldInfoAdapter field)
            {
                builder.EmitOpCode(opcode);
                builder.EmitToken(GetToken(ResolveField(field)), null, DiagnosticBag.GetInstance());
            }

            private void EmitMethod(ILBuilder builder, ILOpCode opcode, ITypeAdapter declaringType, string name, ITypeAdapter[] parameterTypes)
            {
                var symbol = ResolveMethod(declaringType, name, parameterTypes);
                
                builder.EmitOpCode(opcode,
                    opcode == ILOpCode.Newobj
                        ? GetNewobjStackAdjustment(symbol.Parameters.Length)
                        : GetStackAdjustment(symbol, symbol.Parameters.Length));
                
                builder.EmitToken(GetToken(symbol), null, DiagnosticBag.GetInstance());
            }

            private void EmitType(ILBuilder builder, ILOpCode opcode, ITypeAdapter type)
            {
                builder.EmitOpCode(opcode);
                builder.EmitToken(GetToken(ResolveType(type)), null, DiagnosticBag.GetInstance());
            }
            
            private int GetStackAdjustment(MethodSymbol symbol, int argCount)
            {
                var stackAdjustment = 0;
                if (!symbol.ReturnsVoid)
                    stackAdjustment++;
        
                if (symbol.RequiresInstanceReceiver)
                    stackAdjustment--;
        
                stackAdjustment -= argCount;

                return stackAdjustment;
            }
            
            private int GetNewobjStackAdjustment(int argCount)
            {
                return 1 - argCount;
            }
            
            private TypeSymbol ResolveType(ITypeAdapter type)
            {
                return _moduleBuilder
                    .Compilation
                    .Assembly
                    .GetType(type);
            }
            
            private FieldSymbol ResolveField(IFieldInfoAdapter field)
            {
                var netType = ResolveType(field.FieldType);
                return netType.GetMembers(field.Name)[0] as FieldSymbol;
            }
            
            private MethodSymbol ResolveMethod(ITypeAdapter declaringType, string methodName, ITypeAdapter[] paramsTypes)
            {
                bool SelectMethodByParamsExact(Symbol symbol)
                {
                    var parameters = symbol.GetParameters();
                    var parameterTypes = parameters.Select(x => x.Type.Name);

                    return parameterTypes.SequenceEqual(paramsTypes.Select(x => x.Name));
                }

                bool SelectMethodByParamsWithCastChecks(Symbol symbol)
                {
                    var parameters = symbol.GetParameters();
                    var parameterTypes = parameters.Select(x => x.Type).ToArray();
                    var useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>();
                    
                    return parameterTypes
                        .Zip(
                            paramsTypes, 
                            (param, arg) => (param, arg: ResolveType(arg))
                        )
                        .All(t => t.arg.IsEqualToOrDerivedFrom(t.param, TypeCompareKind.ConsiderEverything, ref useSiteInfo));
                }

                var typeSymbol = ResolveType(declaringType);
                var members = typeSymbol
                    .GetMembers(methodName)
                    .Where(symbol => symbol.GetParameters().Length == paramsTypes.Length)
                    .ToArray();

                while (members.IsEmpty())
                {
                    typeSymbol = typeSymbol.BaseTypeNoUseSiteDiagnostics;
                    members = typeSymbol
                        .GetMembers(methodName)
                        .Where(symbol => symbol.GetParameters().Length == paramsTypes.Length)
                        .ToArray();
                }
        
                var method = members.FirstOrDefault(SelectMethodByParamsExact) ?? members.FirstOrDefault(SelectMethodByParamsWithCastChecks);
                while (method is null)
                {
                    typeSymbol = typeSymbol.BaseTypeNoUseSiteDiagnostics;
                    members = typeSymbol
                        .GetMembers(methodName)
                        .Where(symbol => symbol.GetParameters().Length == paramsTypes.Length)
                        .ToArray();
            
                    method = members.FirstOrDefault(SelectMethodByParamsExact) ?? members.FirstOrDefault(SelectMethodByParamsWithCastChecks);
                }

                return method as MethodSymbol;
            }

            private ISignature GetToken(MethodSymbol method)
            {
                return _moduleBuilder.Translate(method, null, DiagnosticBag.GetInstance());
            }
    
            private ITypeReference GetToken(TypeSymbol type)
            {
                return _moduleBuilder.Translate(type, null, DiagnosticBag.GetInstance());
            }
    
            private IFieldReference GetToken(FieldSymbol type)
            {
                return _moduleBuilder.Translate(type, null, DiagnosticBag.GetInstance());
            }

            private LocalDefinition GetLocalDefinition(int index)
            {
                return _localsByIndex[index];
            }

            private LocalDefinition GetLocalDefinition(ILocalBuilderAdapter builder)
            {
                return _localsByBuilder[builder];
            }
        }
    }
}