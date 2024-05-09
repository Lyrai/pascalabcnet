using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    public partial class RoslynILGeneratorAdapter : IILGeneratorAdapter
    {
        private List<Instruction> _instructions = new List<Instruction>();
        private List<object> _arguments = new List<object>();
        private int _labelsCount;
        private ConstructorInfo _labelCtor;

        public RoslynILGeneratorAdapter()
        {
            _labelCtor = typeof(Label).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof(int)}, null);
        }

        public void Emit(OpCode opcode)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, null));
        }

        public void Emit(OpCode opcode, byte arg)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, arg));
        }

        public void Emit(OpCode opcode, sbyte arg)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, arg));
        }

        public void Emit(OpCode opcode, int arg)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, arg));
        }

        public void Emit(OpCode opcode, long arg)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, arg));
        }

        public void Emit(OpCode opcode, string arg)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, arg));
        }

        public void Emit(OpCode opcode, float arg)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, arg));
        }

        public void Emit(OpCode opcode, double arg)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, arg));
        }

        public void Emit(OpCode opcode, IFieldInfoAdapter field)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, field));
        }

        public void Emit(OpCode opcode, IMethodInfoAdapter method)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, method));
        }

        public void Emit(OpCode opcode, IConstructorInfoAdapter constructor)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, constructor));
        }

        public void Emit(OpCode opcode, ILocalBuilderAdapter local)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, local));
        }

        public void Emit(OpCode opcode, ITypeAdapter type)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, type));
        }

        public void Emit(OpCode opcode, Label label)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, label));
        }

        public void Emit(OpCode opcode, Label[] label)
        {
            _instructions.Add(Instruction.Emit);
            _arguments.Add(new EmitInstruction(opcode, label));
        }

        public void EmitCall(OpCode opcode, IMethodInfoAdapter method, ITypeAdapter[] parameterTypesOpt)
        {
            _instructions.Add(Instruction.EmitCall);
            _arguments.Add(new EmitCallInstruction(opcode, method, parameterTypesOpt));
        }

        public ILocalBuilderAdapter DeclareLocal(ITypeAdapter type, bool pinned = false)
        {
            _instructions.Add(Instruction.DeclareLocal);
            var builder = new RoslynLocalBuilderAdapter(type, pinned);
            _arguments.Add(builder);

            return builder;
        }

        public void MarkSequencePoint(ISymbolDocumentWriter document, int startLine, int startColumn, int endLine, int endColumn)
        {
            _instructions.Add(Instruction.MarkSequencePoint);
            _arguments.Add(null);
        }

        public Label DefineLabel()
        {
            _instructions.Add(Instruction.DefineLabel);
            var label = CreateLabel();
            _arguments.Add(label);
            return label;
        }

        public void MarkLabel(Label label)
        {
            _instructions.Add(Instruction.MarkLabel);
            _arguments.Add(label);
        }

        public Label BeginExceptionBlock()
        {
            _instructions.Add(Instruction.BeginExceptionBlock);
            var label = CreateLabel();
            _arguments.Add(label);
            return label;
        }

        public void BeginCatchBlock(ITypeAdapter type)
        {
            _instructions.Add(Instruction.BeginCatchBlock);
            _arguments.Add(type);
        }

        public void BeginFinallyBlock()
        {
            _instructions.Add(Instruction.BeginFinallyBlock);
            _arguments.Add(null);
        }

        public void EndExceptionBlock()
        {
            _instructions.Add(Instruction.EndExceptionBlock);
            _arguments.Add(null);
        }

        internal ILBuilder Realize(PEModuleBuilder moduleBuilder, OptimizationLevel optimizationLevel, bool isVoid)
        {
            Debug.Assert(_instructions.Count == _arguments.Count);
            var converter = new ILConverter(_instructions, _arguments);

            return converter.Realize(moduleBuilder, optimizationLevel, isVoid);
        }

        private Label CreateLabel()
        {
            var label = (Label) _labelCtor.Invoke(new object[] {_labelsCount});
            ++_labelsCount;
            return label;
        }

        private enum Instruction
        {
            Emit,
            EmitCall,
            DeclareLocal,
            MarkSequencePoint,
            DefineLabel,
            MarkLabel,
            BeginExceptionBlock,
            BeginCatchBlock,
            BeginFinallyBlock,
            EndExceptionBlock
        }

        private class EmitInstruction
        {
            public OpCode OpCode { get; }
            public object Argument { get; }

            public EmitInstruction(OpCode opcode, object arg)
            {
                OpCode = opcode;
                Argument = arg;
            }
        }

        private class EmitCallInstruction
        {
            public OpCode OpCode { get; }
            public IMethodInfoAdapter Method { get; }
            public ITypeAdapter[] ParameterTypes { get; }

            public EmitCallInstruction(OpCode opCode, IMethodInfoAdapter method, ITypeAdapter[] parameterTypes)
            {
                OpCode = opCode;
                Method = method;
                ParameterTypes = parameterTypes;
            }
        }
    }
}