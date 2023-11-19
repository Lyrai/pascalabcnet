using System;
using System.Reflection.Emit;

namespace NETGenerator.Adapters
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
        void Emit(OpCode opcode, IConstructorInfoAdapter method);
        void Emit(OpCode opcode, ILocalBuilderAdapter method);
        void Emit(OpCode opcode, TypeAdapter type);
        void EmitCall(OpCode opcode, IMethodInfoAdapter method, TypeAdapter[] parameterTypesOpt);

        ILocalBuilderAdapter DeclareLocal(TypeAdapter type);
    }
}