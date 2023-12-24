using System;
using System.Diagnostics.SymbolStore;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public interface IILGeneratorAdapter
    {
        void Emit(OpCode opcode);
        void Emit(OpCode opcode, byte arg);
        void Emit(OpCode opcode, sbyte arg);
        void Emit(OpCode opcode, int arg);
        void Emit(OpCode opcode, long arg);
        void Emit(OpCode opcode, string arg);
        void Emit(OpCode opcode, float arg);
        void Emit(OpCode opcode, double arg);
        void Emit(OpCode opcode, IFieldInfoAdapter field);
        void Emit(OpCode opcode, IMethodInfoAdapter method);
        void Emit(OpCode opcode, IConstructorInfoAdapter constructor);
        void Emit(OpCode opcode, ILocalBuilderAdapter local);
        void Emit(OpCode opcode, ITypeAdapter type);
        void Emit(OpCode opcode, Label label);
        void Emit(OpCode opcode, Label[] label);
        void EmitCall(OpCode opcode, IMethodInfoAdapter method, ITypeAdapter[] parameterTypesOpt);
        ILocalBuilderAdapter DeclareLocal(ITypeAdapter type, bool pinned = false);
        void MarkSequencePoint(ISymbolDocumentWriter document, int startLine, int startColumn, int endLine, int endColumn);
        Label DefineLabel();
        void MarkLabel(Label label);
        Label BeginExceptionBlock();
        void BeginCatchBlock(ITypeAdapter type);
        void BeginFinallyBlock();
        void EndExceptionBlock();
    }
}