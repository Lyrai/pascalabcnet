// Copyright (c) Ivan Bondarev, Stanislav Mikhalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using PascalABCCompiler.NETGenerator.Adapters;

namespace PascalABCCompiler.NETGenerator
{
    public class TypeFactory
    {
        public static TypeInfo int_type;
        public static TypeInfo double_type;
        public static TypeInfo bool_type;
        public static TypeInfo char_type;
        public static TypeInfo string_type;
        public static TypeInfo byte_type;
        public static TypeAdapter ExceptionType = typeof(Exception).GetAdapter();
        public static TypeAdapter VoidType =   typeof(void).GetAdapter();
        public static TypeAdapter StringType = typeof(string).GetAdapter();
        public static TypeAdapter ObjectType = typeof(object).GetAdapter();
        public static TypeAdapter MonitorType = typeof(System.Threading.Monitor).GetAdapter();
        public static TypeAdapter IntPtr = typeof(System.IntPtr).GetAdapter();
        public static TypeAdapter ArrayType = typeof(System.Array).GetAdapter();
        public static TypeAdapter MulticastDelegateType = typeof(MulticastDelegate).GetAdapter();
        public static TypeAdapter DefaultMemberAttributeType = typeof(DefaultMemberAttribute).GetAdapter();
        public static TypeAdapter EnumType = typeof(Enum).GetAdapter();
        public static TypeAdapter ExtensionAttributeType = typeof(System.Runtime.CompilerServices.ExtensionAttribute).GetAdapter();
        public static TypeAdapter ConvertType = typeof(Convert).GetAdapter();

        //primitive
        public static TypeAdapter BoolType = typeof(Boolean).GetAdapter();
        public static TypeAdapter SByteType = typeof(SByte).GetAdapter();
        public static TypeAdapter ByteType = typeof(Byte).GetAdapter();
        public static TypeAdapter CharType = typeof(Char).GetAdapter();
        public static TypeAdapter Int16Type = typeof(Int16).GetAdapter();
        public static TypeAdapter Int32Type = typeof(Int32).GetAdapter();
        public static TypeAdapter Int64Type = typeof(Int64).GetAdapter();
        public static TypeAdapter UInt16Type = typeof(UInt16).GetAdapter();
        public static TypeAdapter UInt32Type = typeof(UInt32).GetAdapter();
        public static TypeAdapter UInt64Type = typeof(UInt64).GetAdapter();
        public static TypeAdapter SingleType = typeof(Single).GetAdapter();
        public static TypeAdapter DoubleType = typeof(Double).GetAdapter();
        public static TypeAdapter GCHandleType = typeof(GCHandle).GetAdapter();
        public static TypeAdapter MarshalType = typeof(Marshal).GetAdapter();
        public static TypeAdapter TypeType =   typeof(Type).GetAdapter();
        public static TypeAdapter ValueType = typeof(ValueType).GetAdapter();
        public static TypeAdapter IEnumerableType = typeof(System.Collections.IEnumerable).GetAdapter();
        public static TypeAdapter IEnumeratorType = typeof(System.Collections.IEnumerator).GetAdapter();
        public static TypeAdapter IDisposableType = typeof(IDisposable).GetAdapter();
        public static TypeAdapter IEnumerableGenericType = typeof(System.Collections.Generic.IEnumerable<>).GetAdapter();
        public static TypeAdapter IEnumeratorGenericType = typeof(System.Collections.Generic.IEnumerator<>).GetAdapter();

        private static Hashtable types;
        private static Hashtable sizes;
        public static IMethodInfoAdapter ArrayCopyMethod;
        public static IMethodInfoAdapter GetTypeFromHandleMethod;
		public static IMethodInfoAdapter ResizeMethod;
        public static IMethodInfoAdapter GCHandleFreeMethod;
		public static IMethodInfoAdapter StringNullOrEmptyMethod;
        public static IMethodInfoAdapter UnsizedArrayCreateMethodTemplate = null;
        public static IMethodInfoAdapter GCHandleAlloc;
        public static IMethodInfoAdapter GCHandleAllocPinned;
        public static IMethodInfoAdapter OffsetToStringDataProperty;
        public static IMethodInfoAdapter StringLengthMethod;
        public static IMethodInfoAdapter CharToString;
        public static IConstructorInfoAdapter IndexOutOfRangeConstructor;
        public static IConstructorInfoAdapter ParamArrayAttributeConstructor;
        public static IMethodInfoAdapter StringCopyMethod;

        public static IMethodInfoAdapter GetUnsizedArrayCreateMethod(TypeInfo ti)
        {
            if (UnsizedArrayCreateMethodTemplate == null)
                UnsizedArrayCreateMethodTemplate = ArrayType.GetMethod("Resize");
            return UnsizedArrayCreateMethodTemplate.MakeGenericMethod(ti.tp.GetElementType());
        }

        static TypeFactory()
        {
            int_type = new TypeInfo(typeof(int));
            double_type = new TypeInfo(typeof(double));
            bool_type = new TypeInfo(typeof(bool));
            char_type = new TypeInfo(typeof(char));
            string_type = new TypeInfo(typeof(string));
            byte_type = new TypeInfo(typeof(byte));
            
            types = new Hashtable();
            types[BoolType] = BoolType;
            types[SByteType] = SByteType;
            types[ByteType] = ByteType;
            types[CharType] = CharType;
            types[Int16Type] = Int16Type;
            types[Int32Type] = Int32Type;
            types[Int64Type] = Int64Type;
            types[UInt16Type] = UInt16Type;
            types[UInt32Type] = UInt32Type;
            types[UInt64Type] = UInt64Type;
            types[SingleType] = SingleType;
            types[DoubleType] = DoubleType;

            sizes = new Hashtable();
            sizes[BoolType] = sizeof(Boolean);
            sizes[SByteType] = sizeof(SByte);
            sizes[ByteType] = sizeof(Byte);
            sizes[CharType] = sizeof(Char);
            sizes[Int16Type] = sizeof(Int16);
            sizes[Int32Type] = sizeof(Int32);
            sizes[Int64Type] = sizeof(Int64);
            sizes[UInt16Type] = sizeof(UInt16);
            sizes[UInt32Type] = sizeof(UInt32);
            sizes[UInt64Type] = sizeof(UInt64);
            sizes[SingleType] = sizeof(Single);
            sizes[DoubleType] = sizeof(Double);
            //sizes[UIntPtr] = sizeof(UIntPtr);
            
            //types[TypeType] = TypeType;
            ArrayCopyMethod = AdapterFactory.Type(typeof(Array)).GetMethod("Copy", new TypeAdapter[] { typeof(Array).GetAdapter(), typeof(Array).GetAdapter(), typeof(int).GetAdapter() });
            StringNullOrEmptyMethod = AdapterFactory.Type(typeof(string)).GetMethod("IsNullOrEmpty");
            GCHandleAlloc = AdapterFactory.Type(typeof(GCHandle)).GetMethod("Alloc",new TypeAdapter[] {TypeFactory.ObjectType});
            GCHandleAllocPinned = AdapterFactory.Type(typeof(GCHandle)).GetMethod("Alloc", new TypeAdapter[] { TypeFactory.ObjectType, typeof(GCHandleType).GetAdapter() });
            OffsetToStringDataProperty = AdapterFactory.Type(typeof(System.Runtime.CompilerServices.RuntimeHelpers)).GetProperty("OffsetToStringData",BindingFlags.Public|BindingFlags.Static|BindingFlags.Instance).GetGetMethod();
            StringLengthMethod = AdapterFactory.Type(typeof(string)).GetProperty("Length").GetGetMethod();
            IndexOutOfRangeConstructor = AdapterFactory.Type(typeof(IndexOutOfRangeException)).GetConstructor(TypeAdapter.EmptyTypes);
            ParamArrayAttributeConstructor = AdapterFactory.Type(typeof(ParamArrayAttribute)).GetConstructor(TypeAdapter.EmptyTypes);
            GCHandleFreeMethod = AdapterFactory.Type(typeof(GCHandle)).GetMethod("Free");
            GetTypeFromHandleMethod = AdapterFactory.Type(typeof(Type)).GetMethod("GetTypeFromHandle");
            StringCopyMethod = AdapterFactory.Type(typeof(string)).GetMethod("Copy");
            CharToString = AdapterFactory.Type(typeof(char)).GetMethod("ToString", BindingFlags.Static | BindingFlags.Public);
        }

        public static bool IsStandType(TypeAdapter t)
        {
            return types[t] != null;
        }

        public static int GetPrimitiveTypeSize(TypeAdapter PrimitiveType)
        {
            return (int)sizes[PrimitiveType];
        }
    }

    class NETGeneratorTools
    {
        public static void PushStind(IILGeneratorAdapter il, TypeAdapter elem_type)
        {
            switch (TypeAdapter.GetTypeCode(elem_type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte:
                    il.Emit(OpCodes.Stind_I1);
                    break;
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    il.Emit(OpCodes.Stind_I2);
                    break;
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Stind_I4);
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Stind_I8);
                    break;
                case TypeCode.Single:
                    il.Emit(OpCodes.Stind_R4);
                    break;
                case TypeCode.Double:
                    il.Emit(OpCodes.Stind_R8);
                    break;
                default:
                    if (IsPointer(elem_type))
                        il.Emit(OpCodes.Stind_I);
                    else if (elem_type.IsGenericParameter)
                        il.Emit(OpCodes.Stobj, elem_type);
                    else if (IsEnum(elem_type))
                        il.Emit(OpCodes.Stind_I4);
                    else
                        if (elem_type.IsValueType)
                            il.Emit(OpCodes.Stobj, elem_type);
                        else
                            il.Emit(OpCodes.Stind_Ref);
                    break;
            }
        }
        
        public static void PushStelem(IILGeneratorAdapter il,TypeAdapter elem_type)
        {
            switch (TypeAdapter.GetTypeCode(elem_type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte:
                    il.Emit(OpCodes.Stelem_I1);
                    break;
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    il.Emit(OpCodes.Stelem_I2);
                    break;
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Stelem_I4);
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Stelem_I8);
                    break;
                case TypeCode.Single:
                    il.Emit(OpCodes.Stelem_R4);
                    break;
                case TypeCode.Double:
                    il.Emit(OpCodes.Stelem_R8);
                    break;
                default:
                    if (IsPointer(elem_type))
                        il.Emit(OpCodes.Stelem_I);
                    else if (elem_type.IsGenericParameter)
                        il.Emit(OpCodes.Stelem, elem_type);
                    else if (IsEnum(elem_type))
                        il.Emit(OpCodes.Stelem_I4);
                    else 
                        if (elem_type.IsValueType) 
                            il.Emit(OpCodes.Stobj, elem_type);
                        else 
                            il.Emit(OpCodes.Stelem_Ref);
                    break;
            }
        }

        public static void PushParameterDereference(IILGeneratorAdapter il, TypeAdapter elem_type)
        {
            switch (TypeAdapter.GetTypeCode(elem_type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                    il.Emit(OpCodes.Ldind_U1);
                    break;
                case TypeCode.SByte:
                    il.Emit(OpCodes.Ldind_I1);
                    break;
                case TypeCode.Char:
                case TypeCode.UInt16:
                    il.Emit(OpCodes.Ldind_U2);
                    break;
                case TypeCode.Int16:
                    il.Emit(OpCodes.Ldind_I2);
                    break;
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Ldind_U4);
                    break;
                case TypeCode.Int32:
                    il.Emit(OpCodes.Ldind_I4);
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Ldind_I8);
                    break;
                case TypeCode.Single:
                    il.Emit(OpCodes.Ldind_R4);
                    break;
                case TypeCode.Double:
                    il.Emit(OpCodes.Ldind_R8);
                    break;
                default:
                    if (IsPointer(elem_type))
                        il.Emit(OpCodes.Ldind_I);
                    else
                        if (elem_type.IsValueType || elem_type.IsGenericParameter)
                            il.Emit(OpCodes.Ldobj, elem_type);
                        else
                            il.Emit(OpCodes.Ldind_Ref);
                    break;
            }
        }

        public static void PushLdelem(IILGeneratorAdapter il, TypeAdapter elem_type, bool ldobj)
        {
            switch (TypeAdapter.GetTypeCode(elem_type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                    il.Emit(OpCodes.Ldelem_U1);
                    break;
                case TypeCode.SByte:
                    il.Emit(OpCodes.Ldelem_I1);
                    break;
                case TypeCode.Char:
                case TypeCode.Int16:
                    il.Emit(OpCodes.Ldelem_I2);
                    break;
                case TypeCode.UInt16:
                    il.Emit(OpCodes.Ldelem_U2);
                    break;
                case TypeCode.Int32:
                    il.Emit(OpCodes.Ldelem_I4);
                    break;
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Ldelem_U4);
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Ldelem_I8);
                    break;
                case TypeCode.Single:
                    il.Emit(OpCodes.Ldelem_R4);
                    break;
                case TypeCode.Double:
                    il.Emit(OpCodes.Ldelem_R8);
                    break;
                default:
                    if (IsPointer(elem_type))
                        il.Emit(OpCodes.Ldelem_I);
                    else if (elem_type.IsGenericParameter)
                        il.Emit(OpCodes.Ldelem, elem_type);
                    else
                        if (elem_type.IsValueType)//если это структура
                        {
                            il.Emit(OpCodes.Ldelema, elem_type);//почему a?
                            // проверки нужно ли заменять тип возвр. знач. метода get_val массива на указатель
                            if (ldobj || !(elem_type != TypeFactory.VoidType && elem_type.IsValueType && !TypeFactory.IsStandType(elem_type)))
                                il.Emit(OpCodes.Ldobj, elem_type);
                        }
                        else il.Emit(OpCodes.Ldelem_Ref);
                    break;
            }           
        }
        public static void LdcIntConst(IILGeneratorAdapter il, int e)
        {
            switch (e)
            {
                case -1: il.Emit(OpCodes.Ldc_I4_M1); break;
                case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if (e < sbyte.MinValue || e > sbyte.MaxValue)
                        il.Emit(OpCodes.Ldc_I4, e);
                    else
                        il.Emit(OpCodes.Ldc_I4_S, (sbyte)e);
                    break;
                /*if (e > sbyte.MinValue && e < sbyte.MaxValue)  //DarkStar Changed
                    il.Emit(OpCodes.Ldc_I4_S,(sbyte)e);
                else if (e > Int32.MinValue && e < Int32.MaxValue)  
                    il.Emit(OpCodes.Ldc_I4, (int)e); break;		*/
            }
        }

        public static void PushLdc(IILGeneratorAdapter il, TypeAdapter elem_type, object value)
        {
            switch (TypeAdapter.GetTypeCode(elem_type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                    //il.Emit(OpCodes.Ldc_I4_S, Convert.ToByte(value));
                    LdcIntConst(il, Convert.ToByte(value));
                    break;
                case TypeCode.SByte:
                    LdcIntConst(il, Convert.ToSByte(value));
                    //il.Emit(OpCodes.Ldc_I4_S, Convert.ToSByte(value));
                    break;
                case TypeCode.Char:
                    LdcIntConst(il, Convert.ToChar(value));
                    //il.Emit(OpCodes.Ldc_I4, Convert.ToChar(value));
                    break;
                case TypeCode.Int16:
                    LdcIntConst(il, Convert.ToInt32(value));
                    //il.Emit(OpCodes.Ldc_I4, Convert.ToInt32(value));
                    break;
                case TypeCode.UInt16:
                    LdcIntConst(il, Convert.ToUInt16(value));
                    //il.Emit(OpCodes.Ldc_I4, Convert.ToUInt16(value));
                    break;
                case TypeCode.Int32:
                    LdcIntConst(il,Convert.ToInt32(value));
                    break;
                case TypeCode.UInt32:
                    LdcIntConst(il, (Int32)Convert.ToUInt32(value));
                    //il.Emit(OpCodes.Ldc_I4, Convert.ToUInt32(value));
                    break;
                case TypeCode.Int64:
                    il.Emit(OpCodes.Ldc_I8, Convert.ToInt64(value));
                    break;
                case TypeCode.UInt64:
                    UInt64 UInt64 = Convert.ToUInt64(value);
                    if (UInt64 > Int64.MaxValue)
                    {
                        //Это будет медленно работать. Надо переделать.
                        //Надо разобраться как сссделано в C#, там все нормально
                        Int64 tmp = (Int64)(UInt64 - Int64.MaxValue - 1);
                        il.Emit(OpCodes.Ldc_I8, tmp);
                        il.Emit(OpCodes.Conv_U8);
                        il.Emit(OpCodes.Ldc_I8, Int64.MaxValue);
                        il.Emit(OpCodes.Conv_U8);
                        il.Emit(OpCodes.Add);
                        il.Emit(OpCodes.Ldc_I4_1);
                        il.Emit(OpCodes.Add);
                    }
                    else
                        il.Emit(OpCodes.Ldc_I8, Convert.ToInt64(value));
                    break;
                case TypeCode.Single:
                    il.Emit(OpCodes.Ldc_R4, (Single)value);
                    break;
                case TypeCode.Double:
                    il.Emit(OpCodes.Ldc_R8, (Double)value);
                    break;
                case TypeCode.String:
                    il.Emit(OpCodes.Ldstr, (string)value);
                    break;
                default:
                    if (IsEnum(elem_type))
                        //il.Emit(OpCodes.Ldc_I4, (Int32)value);
                        LdcIntConst(il, (Int32)value);
                    else
                        throw new Exception("Немогу положить PushLdc для " + value.GetType().ToString());
                    break;
            }
        }

        public static void PushCast(IILGeneratorAdapter il, TypeAdapter tp, TypeAdapter from_value_type)
        {
            if (IsPointer(tp))
                return;
            //(ssyy) Вставил 15.05.08
            if (from_value_type is object)
            {
                il.Emit(OpCodes.Box, from_value_type);
            }
            if (tp.IsValueType || tp.IsGenericParameter)
                il.Emit(OpCodes.Unbox_Any, tp);
            else
                il.Emit(OpCodes.Castclass, tp);
        }
        
        public static ILocalBuilderAdapter CreateLocalAndLoad(IILGeneratorAdapter il, TypeAdapter tp)
        {
            ILocalBuilderAdapter lb = il.DeclareLocal(tp);
            il.Emit(OpCodes.Stloc, lb);
            if (tp.IsValueType)
                il.Emit(OpCodes.Ldloca, lb);
            else
                il.Emit(OpCodes.Ldloc, lb);
            return lb;
        }
        
        public static ILocalBuilderAdapter CreateLocal(IILGeneratorAdapter il, TypeAdapter tp)
        {
            ILocalBuilderAdapter lb = il.DeclareLocal(tp);
            il.Emit(OpCodes.Stloc, lb);
            return lb;
        }
        
        public static ILocalBuilderAdapter CreateLocalAndLdloca(IILGeneratorAdapter il, TypeAdapter tp)
        {
            ILocalBuilderAdapter lb = il.DeclareLocal(tp);
            il.Emit(OpCodes.Stloc, lb);
            il.Emit(OpCodes.Ldloca, lb);
            return lb;
        }

        public static void CreateBoundedArray(IILGeneratorAdapter il, IFieldBuilderAdapter fb, TypeInfo ti)
        {
            Label lbl = il.DefineLabel();
            if (fb.IsStatic)
                il.Emit(OpCodes.Ldsfld, fb);
            else
            {
                //il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fb);
            }
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Brfalse, lbl);
            if (!fb.IsStatic)
                il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ti.def_cnstr);
            if (fb.IsStatic)
                il.Emit(OpCodes.Stsfld, fb);
            else
                il.Emit(OpCodes.Stfld, fb);
            il.MarkLabel(lbl);
        }

        public static void CreateBoudedArray(IILGeneratorAdapter il, ILocalBuilderAdapter lb, TypeInfo ti)
        {
            Label lbl = il.DefineLabel();
            il.Emit(OpCodes.Ldloc, lb);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Brfalse, lbl);
            il.Emit(OpCodes.Newobj, ti.def_cnstr);
            il.Emit(OpCodes.Stloc, lb);
            il.MarkLabel(lbl);
        }

        public static bool IsBoundedArray(TypeInfo ti)
        {
            return ti.arr_fld != null;
        }

        public static void FixField(IMethodBuilderAdapter mb, IFieldBuilderAdapter fb, TypeInfo ti)
        {
            IILGeneratorAdapter il = mb.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fb);
            if (fb.FieldType == TypeFactory.StringType)
            {
                il.Emit(OpCodes.Ldc_I4, (int)GCHandleType.Pinned);
                il.Emit(OpCodes.Call, TypeFactory.GCHandleAllocPinned);
            }
            else
            {
                il.Emit(OpCodes.Call, TypeFactory.GCHandleAlloc);
            }
            il.Emit(OpCodes.Pop);
        }

        public static void CloneField(IMethodBuilderAdapter clone_meth, IFieldBuilderAdapter fb, TypeInfo ti)
        {
            IILGeneratorAdapter il = clone_meth.GetILGenerator();
            il.Emit(OpCodes.Ldloca_S, (byte)0);
            il.Emit(OpCodes.Ldarg_0);
            if (ti.clone_meth != null)
            {
                if (fb.FieldType.IsValueType)
                    il.Emit(OpCodes.Ldflda, fb);
                else
                    il.Emit(OpCodes.Ldfld, fb);
                il.Emit(OpCodes.Call, ti.clone_meth);
            }
            else
            {
                il.Emit(OpCodes.Ldfld, fb);
            }
            il.Emit(OpCodes.Stfld, fb);
        }

        public static void AssignField(IMethodBuilderAdapter ass_meth, IFieldBuilderAdapter fb, TypeInfo ti)
        {
            IILGeneratorAdapter il = ass_meth.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarga_S, (byte)1);
            if (ti.clone_meth != null)
            {
                if (fb.FieldType.IsValueType)
                    il.Emit(OpCodes.Ldflda, fb);
                else
                    il.Emit(OpCodes.Ldfld, fb);
                il.Emit(OpCodes.Call, ti.clone_meth);
            }
            else
            {
                il.Emit(OpCodes.Ldfld, fb);
            }
            il.Emit(OpCodes.Stfld, fb);
        }

        public static void PushTypeOf(IILGeneratorAdapter il, TypeAdapter tp)
        {
            il.Emit(OpCodes.Ldtoken, tp);
            il.EmitCall(OpCodes.Call, TypeFactory.GetTypeFromHandleMethod, null);
        }
        
        public static bool IsPointer(TypeAdapter tp)
        {
            return tp.IsPointer; /*|| tp==TypeFactory.IntPtr; INTPTR TODO*/
        }

        public static bool IsEnum(TypeAdapter tp)
        {
            return !tp.IsGenericType && !tp.IsGenericTypeDefinition && tp.IsEnum;
        }
        
    }
}
