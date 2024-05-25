using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE;
using NETGenerator.Adapters.Utility;

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
            private MethodSymbol _method;

            public ILConverter(List<Instruction> instructions, List<object> arguments, MethodSymbol method)
            {
                _instructions = instructions;
                _arguments = arguments;
                _method = method;
                
                _labels = new Dictionary<int, object>();
                _localsByBuilder = new Dictionary<ILocalBuilderAdapter, LocalDefinition>();
                _localsByIndex = new Dictionary<int, LocalDefinition>();
            }

            public ILBuilder Realize(PEModuleBuilder moduleBuilder, OptimizationLevel optimizationLevel, bool isVoid)
            {
                _moduleBuilder = moduleBuilder;
                ILBuilder builder = new ILBuilder(moduleBuilder, new LocalSlotManager(null), optimizationLevel, true);
                var argument = _arguments.GetEnumerator();
                argument.MoveNext();
                object currentExceptionLabel = null;
                
                foreach (var instruction in _instructions)
                {
                    switch (instruction)
                    {
                        case Instruction.Emit:
                            var emitArg = argument.Current as EmitInstruction;
                            var opcode = GetOpCode(emitArg.OpCode);
                            if (emitArg.Argument is null)
                            {
                                if (opcode == ILOpCode.Ret)
                                {
                                    builder.EmitRet(isVoid);
                                }
                                else if (opcode == ILOpCode.Throw)
                                {
                                    builder.EmitThrow(false);
                                }
                                else
                                {
                                    builder.EmitOpCode(opcode);
                                }
                                
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
                                case IMethodBaseAdapter method:
                                    EmitMethod(builder, opcode, method);
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
                            var localType = GetToken(localArg.LocalType);
                            var definition = builder.LocalSlotManager.AllocateSlot(localType, LocalSlotConstraints.None);
                            _localsByBuilder.Add(localArg, definition);
                            _localsByIndex.Add(definition.SlotIndex, definition);
                            break;
                        case Instruction.EmitCall:
                            var emitCallArgument = argument.Current as EmitCallInstruction;
                            var m = emitCallArgument.Method;
                            EmitMethod(builder, GetOpCode(emitCallArgument.OpCode), m);
                            break;
                        case Instruction.MarkSequencePoint:
                            break;
                        case Instruction.DefineLabel:
                            var labelArgument = (Label) argument.Current;
                            _labels.Add(GetLabelNum(labelArgument), new object());
                            break;
                        case Instruction.MarkLabel:
                            var labelArgument1 = (Label) argument.Current;
                            builder.MarkLabel(_labels[GetLabelNum(labelArgument1)]);
                            break;
                        case Instruction.BeginExceptionBlock:
                            var tryLabel = (Label) argument.Current;
                            var tryLabelObject = new object();
                            _labels.Add(GetLabelNum(tryLabel), tryLabelObject);
                            currentExceptionLabel = tryLabelObject;
                            builder.OpenLocalScope(ScopeType.TryCatchFinally);
                            builder.OpenLocalScope(ScopeType.Try);
                            break;
                        case Instruction.BeginCatchBlock:
                            var exceptionType = argument.Current as ITypeAdapter;
                            builder.CloseLocalScope();
                            builder.AdjustStack(1);
                            builder.OpenLocalScope(ScopeType.Catch, GetToken(exceptionType));
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

                    argument.MoveNext();
                }
                
                builder.Realize();
                builder.FreeBasicBlocks();
                argument.Dispose();
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
                int arg;
                if (argument is int)
                {
                    arg = (int) argument;
                }
                else
                {
                    arg = (byte) argument;
                }
                
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
                builder.EmitToken(GetToken(field), null, DiagnosticBag.GetInstance());
            }

            private void EmitMethod(ILBuilder builder, ILOpCode opcode, IMethodBaseAdapter method)
            {
                var type = ResolveHelper.ResolveType(method.DeclaringType);
                type = FixGenericTypeArgumentsIfNeeded(method.DeclaringType, type);
                var symbol = ResolveHelper.ResolveMethod(method, type);
                //var symbol = ResolveHelper.ResolveMethod(method);
                var stackAdjustment = 0;
                if (opcode == ILOpCode.Newobj)
                {
                    stackAdjustment = GetNewobjStackAdjustment(symbol.Parameters.Length);
                }
                else if (opcode.HasVariableStackBehavior())
                {
                    stackAdjustment = GetStackAdjustment(symbol, symbol.Parameters.Length);
                }
                else
                {
                    stackAdjustment = opcode.NetStackBehavior();
                }
                
                builder.EmitOpCode(opcode, stackAdjustment);
                
                builder.EmitToken(GetToken(symbol), null, DiagnosticBag.GetInstance());
            }

            private void EmitType(ILBuilder builder, ILOpCode opcode, ITypeAdapter type)
            {
                builder.EmitOpCode(opcode);
                builder.EmitToken(GetToken(type), null, DiagnosticBag.GetInstance());
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
            
            private ISignature GetToken(MethodSymbol symbol)
            {
                return _moduleBuilder.Translate(symbol, null, DiagnosticBag.GetInstance());
            }
    
            private ITypeReference GetToken(ITypeAdapter type)
            {
                var symbol = ResolveHelper.ResolveType(type);
                symbol = FixGenericTypeArgumentsIfNeeded(type, symbol);
                return _moduleBuilder.Translate(symbol, null, DiagnosticBag.GetInstance());
            }

            private TypeSymbol FixGenericTypeArgumentsIfNeeded(ITypeAdapter type, TypeSymbol symbol)
            {
                if (type is RoslynGenericTypeAdapter generic && !generic.Adaptee.IsUnboundGenericType)
                {
                    var types = new List<TypeWithAnnotations>();
                    bool flag = false;
                    foreach (var typeArg in generic.Adaptee.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics)
                    {
                        if (typeArg.Type is PascalTypeParameterSymbol param && param.ContainingSymbol is null)
                        {
                            var methodParameter = _method
                                .TypeParameters
                                .First(methParam => methParam.Name == param.Name);

                            if (!(methodParameter is null))
                            {
                                flag = true;
                            }
                            
                            types.Add(TypeWithAnnotations.Create(methodParameter));
                        }
                        else
                        {
                            types.Add(typeArg);
                        }
                    }

                    if(flag)
                    {
                        symbol = (symbol.ConstructedFrom() as NamedTypeSymbol).ConstructIfGeneric(types.ToImmutableArray());
                    }
                }

                return symbol;
            }
    
            private IFieldReference GetToken(IFieldInfoAdapter field)
            {
                var type = ResolveHelper.ResolveType(field.DeclaringType);
                type = FixGenericTypeArgumentsIfNeeded(field.DeclaringType, type);
                var symbol = ResolveHelper.ResolveField(field, type);
                return _moduleBuilder.Translate(symbol, null, DiagnosticBag.GetInstance());
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