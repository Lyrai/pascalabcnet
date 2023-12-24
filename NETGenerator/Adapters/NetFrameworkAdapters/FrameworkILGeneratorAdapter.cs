using System;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters
{
    public class FrameworkILGeneratorAdapter: IILGeneratorAdapter
    {
        public ILGenerator Adaptee { get; }

        public FrameworkILGeneratorAdapter(ILGenerator generator)
        {
            Adaptee = generator;
        }
        
        public void Emit(OpCode opcode)
        {
            Adaptee.Emit(opcode);
        }

        public void Emit(OpCode opcode, byte arg)
        {
            Adaptee.Emit(opcode, arg);
        }

        public void Emit(OpCode opcode, sbyte arg)
        {
            Adaptee.Emit(opcode, arg);
        }

        public void Emit(OpCode opcode, int arg)
        {
            Adaptee.Emit(opcode, arg);
        }

        public void Emit(OpCode opcode, long arg)
        {
            Adaptee.Emit(opcode, arg);
        }

        public void Emit(OpCode opcode, string arg)
        {
            Adaptee.Emit(opcode, arg);
        }

        public void Emit(OpCode opcode, float arg)
        {
            Adaptee.Emit(opcode, arg);
        }

        public void Emit(OpCode opcode, double arg)
        {
            Adaptee.Emit(opcode, arg);
        }

        public void Emit(OpCode opcode, IFieldInfoAdapter field)
        {
            Adaptee.Emit(opcode, (field as FrameworkFieldInfoAdapter).Adaptee);
        }

        public void Emit(OpCode opcode, IMethodInfoAdapter method)
        {
            Adaptee.Emit(opcode, (method as FrameworkMethodInfoAdapter).Adaptee);
        }

        public void Emit(OpCode opcode, IConstructorInfoAdapter constructor)
        {
            Adaptee.Emit(opcode, (constructor as FrameworkConstructorInfoAdapter).Adaptee);
        }

        public void Emit(OpCode opcode, ILocalBuilderAdapter local)
        {
            Adaptee.Emit(opcode, (local as FrameworkLocalBuilderAdapter).Adaptee);
        }

        public void Emit(OpCode opcode, ITypeAdapter type)
        {
            Adaptee.Emit(opcode, (type as FrameworkTypeAdapter).Adaptee);
        }

        public void Emit(OpCode opcode, Label label)
        {
            Adaptee.Emit(opcode, label);
        }

        public void Emit(OpCode opcode, Label[] label)
        {
            Adaptee.Emit(opcode, label);
        }

        public void EmitCall(OpCode opcode, IMethodInfoAdapter method, ITypeAdapter[] parameterTypesOpt)
        {
            var types = parameterTypesOpt?.Select(t => (t as FrameworkTypeAdapter).Adaptee).ToArray();;
            Adaptee.EmitCall(opcode, (method as FrameworkMethodInfoAdapter).Adaptee, types);
        }

        public ILocalBuilderAdapter DeclareLocal(ITypeAdapter type, bool pinned = false)
        {
            return Adaptee.DeclareLocal((type as FrameworkTypeAdapter).Adaptee, pinned).GetAdapter();
        }

        public void MarkSequencePoint(ISymbolDocumentWriter document, int startLine, int startColumn, int endLine, int endColumn)
        {
            Adaptee.MarkSequencePoint(document, startLine, startColumn, endLine, endColumn);
        }

        public Label DefineLabel()
        {
            return Adaptee.DefineLabel();
        }

        public void MarkLabel(Label label)
        {
            Adaptee.MarkLabel(label);
        }

        public Label BeginExceptionBlock()
        {
            return Adaptee.BeginExceptionBlock();
        }

        public void BeginCatchBlock(ITypeAdapter type)
        {
            Adaptee.BeginCatchBlock((type as FrameworkTypeAdapter).Adaptee);
        }

        public void BeginFinallyBlock()
        {
            Adaptee.BeginFinallyBlock();
        }

        public void EndExceptionBlock()
        {
            Adaptee.EndExceptionBlock();
        }
    }
}