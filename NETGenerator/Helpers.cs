﻿// Copyright (c) Ivan Bondarev, Stanislav Mikhalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using PascalABCCompiler.SemanticTree;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using PascalABCCompiler.NETGenerator.Adapters;

namespace PascalABCCompiler.NETGenerator {
	
	public class HandlerFactory
	{
		public static IConstructorInfoAdapter ci;
		public static TypeAdapter[] parameters;
		public static TypeAdapter eventHandler;
		
		static HandlerFactory()
		{
			eventHandler = typeof(EventHandler).GetAdapter();
			parameters = new [] { typeof(object).GetAdapter(), typeof(IntPtr).GetAdapter() };
			ci = eventHandler.GetConstructor(parameters);
		}
	}
	
	public abstract class NodeInfo
    {
		
	}

    public static class OperatorsNameConvertor
    {
        private static System.Collections.Generic.Dictionary<string, string> names =
            new System.Collections.Generic.Dictionary<string, string>(32);
        
        static OperatorsNameConvertor()
        {
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.plus_name]="op_Addition";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.minus_name]="op_Subtraction";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.mul_name]="op_Multiply";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.div_name]="op_Division";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.and_name]="op_BitwiseAnd";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.or_name]="op_BitwiseOr";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.eq_name]="op_Equality";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.gr_name]="op_GreaterThan";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.greq_name]="op_GreaterThanOrEqual";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.sm_name]="op_LessThan";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.smeq_name]="op_LessThanOrEqual";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.mod_name]="op_Modulus";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.not_name]="op_LogicalNot";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.noteq_name]="op_Inequality";
            
            //op_Implicit
            //op_Explicit

            names[PascalABCCompiler.TreeConverter.compiler_string_consts.xor_name]="op_ExclusiveOr";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.and_name]="op_LogicalAnd";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.or_name]="op_LogicalOr";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.assign_name]="op_Assign";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.shl_name]="op_LeftShift";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.shr_name]="op_RightShift";
            //names["op_SignedRightShift"]=PascalABCCompiler.TreeConverter.compiler_string_consts.shr_name;
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.shr_name]="op_UnsignedRightShift";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.eq_name]="op_Equality";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.multassign_name]="op_MultiplicationAssignment";
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.minusassign_name]="op_SubtractionAssignment";
            //names[PascalABCCompiler.TreeConverter.compiler_string_consts.minusassign_name]="op_ExclusiveOrAssignment";
            //op_LeftShiftAssignment
            //op_ModulusAssignment
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.plusassign_name]="op_AdditionAssignment";
            //op_BitwiseAndAssignment
            //op_BitwiseOrAssignment
            //op_Comma
            names[PascalABCCompiler.TreeConverter.compiler_string_consts.divassign_name]="op_DivisionAssignment";
            //op_Decrement
            //op_Increment
            //names[PascalABCCompiler.TreeConverter.compiler_string_consts.minus_name] ="op_UnaryNegation";
            //op_UnaryPlus
            //op_OnesComplement

        }

        public static string convert_name(string name)
        {
            string ret;
            if (names.TryGetValue(name, out ret))
            {
                return ret;
            }
            return null;
        }
    }
	
	public class TypeInfo : NodeInfo
    {
		private TypeAdapter _tp;
		private bool _is_arr=false;//флаг массив ли это
		public bool is_set=false;
		public bool is_typed_file=false;
		public bool is_text_file=false;
		public int arr_len;
		public IConstructorInfoAdapter def_cnstr;//конструктор по умолчанию типа (если он есть)
		public IFieldInfoAdapter arr_fld;//ссылка на поле массива в оболочке над массивом
		public IMethodInfoAdapter clone_meth;//метод копирования в массиве
		public IMethodInfoAdapter init_meth;//метод инициализации
        public IMethodInfoAdapter assign_meth;//метод присваивания значений размерных типов
        public IConstructorBuilderAdapter static_cnstr;
        public IMethodBuilderAdapter fix_meth;
		//временно для событий
		public IMethodBuilderAdapter handl_meth;
		public bool has_events=false;//есть ли в типе события
		//public Hashtable fields=new Hashtable();//временно
        public IMethodInfoAdapter enumerator_meth;
		
		public TypeInfo() {}
		
		public TypeInfo(TypeAdapter tp)
		{
			_tp = tp;
		}

		public TypeInfo(Type tp)
		{
			_tp = AdapterFactory.Type(tp);
		}
		
		public TypeAdapter tp {
			get {
				return _tp;
			}
			set {
				_tp = value;
			}
		}
		
		public bool is_arr {
			get 
            {
	            return _is_arr;
			}
			set 
            {
				_is_arr = value;
			}
		}
	}
	
	public class EvntInfo : NodeInfo
	{
		private IEventBuilderAdapter _ei;
		
		public EvntInfo(IEventBuilderAdapter ei)
		{
			_ei = ei;
		}
		
		public EvntInfo(EventBuilder ei)
		{
			_ei = ei.GetAdapter();
		}
		
		public IEventBuilderAdapter ei
        {
			get {
				return _ei;
			}
			set {
				_ei = value;
			}
		}
		
	}
	
	public class FldInfo : NodeInfo {
		private IFieldInfoAdapter _fi;
		
		public FldInfo() {}

        public FldInfo(IFieldInfoAdapter fi)
		{
			_fi = fi;
		}

        public IFieldInfoAdapter fi
        {
			get {
				return _fi;
			}
			set {
				_fi = value;
			}
		}
		
        public virtual TypeAdapter field_type
        {
            get
            {
                return fi.FieldType;
            }
        }
	}

    public class GenericFldInfo : FldInfo
    {
        private TypeAdapter _field_type;
        public IFieldInfoAdapter prev_fi; // передаю чтобы на третьем этапе в NegGenerator.cs (примерно 1586) можно было сконструировать правильный тип. Костыль для #1632

        public override TypeAdapter field_type
        {
            get
            {
                return _field_type;
            }
        }

        public GenericFldInfo(IFieldInfoAdapter fi, TypeAdapter field_type, IFieldInfoAdapter prev_fi)
            : base(fi)
        {
            _field_type = field_type;
            this.prev_fi = prev_fi;
        }
	}
	
    public class PropInfo : NodeInfo {
    	private IPropertyInfoAdapter _prop;
    	
    	public PropInfo(PropertyInfo _prop)
    	{
    		this._prop = AdapterFactory.PropertyInfo(_prop);
    	}
    	
        public PropInfo(IPropertyInfoAdapter _prop)
        {
	        this._prop = _prop;
        }
        
    	public IPropertyInfoAdapter prop
    	{
    		get
    		{
    			return _prop;
    		}
    	}
    }
    
	public class ConstrInfo : NodeInfo {
		private IConstructorInfoAdapter _ci;
		
		public ConstrInfo() {}
		
		public ConstrInfo(IConstructorInfoAdapter ci)
		{
			_ci = ci;
		}
		
		public IConstructorInfoAdapter ci {
			get {
				return _ci;
			}
			set {
				_ci = value;
			}
		}
	}
	
	public class MethInfo : NodeInfo {
		private IMethodInfoAdapter _mi;
		//private ILocalBuilderAdapter _ret_val;//переменная для возвр. значения //(ssyy) Нет пользы
		private ILocalBuilderAdapter _frame;//перем, хранящая запись активации
		private MethInfo _up_meth;//ссылка на верхний метод
		private Frame _disp;//запись активации
		private bool _nested=false;//является ли вложенной или содержащей вложенные
		private int _num_scope;//номер области видимости
		private IConstructorInfoAdapter _cnstr;
		private bool _stand=false;//для станд. процедур, у которого нет тела в семант. дереве ("New","Dispose")
        private bool _is_in_class = false;//является ли он процедурой, влож. в метод
        private bool _is_ptr_ret_type = false;

		public MethInfo() {}
		
		public MethInfo(MethodInfo mi)
		{
			_mi = AdapterFactory.MethodInfo(mi);
		}
		
		public MethInfo(IMethodInfoAdapter mi)
		{
			_mi = mi;
		}

        public bool is_ptr_ret_type
        {
            get
            {
                return _is_ptr_ret_type;
            }
            set
            {
                _is_ptr_ret_type = value;
            }
        }

        public bool is_in_class
        {
            get
            {
                return _is_in_class;
            }
            set
            {
                _is_in_class = value;
            }
        }

		public bool stand
		{
			get
			{
				return _stand;
			}
			set
			{
				_stand = value;
			}
		}
		
		public IMethodInfoAdapter mi {
			get {
				return _mi;
			}
			set {
				_mi = value;
			}
		}
		
		public MethInfo up_meth {
			get {
				return _up_meth;
			}
			set {
				_up_meth = value;
			}
		}
		
		public IConstructorInfoAdapter cnstr {
			get {
				return _cnstr;
			}
			set {
				_cnstr = value;
			}
		}
		
		public int num_scope {
			get {
				return _num_scope;
			}
			set {
				_num_scope = value;
			}
		}
		
		public Frame disp {
			get {
				return _disp;
			}
			set {
				_disp = value;
			}
		}
		
		public bool nested {
			get {
				return _nested;
			}
			set {
				_nested = value;
			}
		}
		
		public ILocalBuilderAdapter frame {
			get {
				return _frame;
			}
			set {
				_frame = value;
			}
		}
		
        //(ssyy) Нет пользы
		/*public ILocalBuilderAdapter ret_val {
			get {
				return _ret_val;
			}
			set {
				_ret_val = value;
			}
		}*/
		
	}
	
	public enum VarKind 
	{
		vkLocal, //локальная 
		vkNonLocal, //нелокальная(содержится в процедуре) 
		vkGlobal //глобальная переменная (основная программа)
	}
	
	public class VarInfo : NodeInfo {
		private ILocalBuilderAdapter _lb;//билдер для переменной
		private IFieldBuilderAdapter _fb;//а вдруг переменная нелокальная
		private VarKind _kind;//тип переменной
		private MethInfo _meth;//метод, в котором определена переменная
		
		public VarInfo() {}
		
		public VarInfo(LocalBuilder lb)
		{
			_lb = AdapterFactory.LocalBuilder(lb);
			_kind = VarKind.vkLocal;
		}
		
		public VarInfo(ILocalBuilderAdapter lb)
		{
			_lb = lb;
			_kind = VarKind.vkLocal;
		}
		
		public MethInfo meth {
			get {
				return _meth;
			}
			set {
				_meth = value;
			}
		}
		
		public IFieldBuilderAdapter fb {
			get {
				return _fb;
			}
			set {
				_fb = value;
			}
		}
		
		public VarKind kind {
			get {
				return _kind;
			}
			set {
				_kind = value;
			}
		}
		
		public ILocalBuilderAdapter lb {
			get {
				return _lb;
			}
			set {
				_lb = value;
			}
		}	
	}
	
	public enum ParamKind {
		pkNone,
		pkGlobal
	}
	
	public class ParamInfo : NodeInfo {
		private IParameterBuilderAdapter _pb;//билдер для параметра
		private IFieldBuilderAdapter _fb;//вдруг параметр нелокальный
		private ParamKind _kind = ParamKind.pkNone;
		private MethInfo _meth;//метод, в котор. описан параметр
		
		public ParamInfo() {}
		
		public ParamInfo(IParameterBuilderAdapter pb)
		{
			_pb = pb;
		}
		
		public ParamInfo(ParameterBuilder pb)
		{
			_pb = pb.GetAdapter();
		}
		
		public MethInfo meth {
			get {
				return _meth;
			}
			set {
				_meth = value;
			}
		}
		
		public ParamKind kind {
			get {
				return _kind;
			}
			set {
				_kind = value;
			}
		}
		
		public IFieldBuilderAdapter fb {
			get {
				return _fb;
			}
			set {
				_fb = value;
			}
		}
		
		public IParameterBuilderAdapter pb {
			get {
				return _pb;
			}
			set {
				_pb = value;
			}
		}	
	}
	
	public class ConstInfo : NodeInfo {
		public IFieldBuilderAdapter fb;
		
		public ConstInfo(FieldBuilder fb)
		{
			this.fb = AdapterFactory.FieldBuilder(fb);
		}
		
		public ConstInfo(IFieldBuilderAdapter fb)
		{
			this.fb = fb;
		}
	}
	
	//Структура для записи активации процедуры
	public class Frame {
		public TypeBuilderAdapter tb; //класс - запись активации
		public IFieldBuilderAdapter parent; //поле-ссылка на род. запись активации
		public IConstructorBuilderAdapter cb; //конструктор записи активации
		public IMethodBuilderAdapter mb;
		
		public Frame() {}
	}
	
	public class Helper {
		public Hashtable defs=new Hashtable();
        private Hashtable processing_types = new Hashtable();
		private IMethodInfoAdapter arr_mi=null;
		private Hashtable pas_defs = new Hashtable();
        private Hashtable memoized_exprs = new Hashtable();
		private Hashtable dummy_methods = new Hashtable();

		public Helper() {}
		
		public void AddDummyMethod(TypeBuilderAdapter tb, IMethodBuilderAdapter mb)
        {
			dummy_methods[tb] = mb;
        }

		public IMethodBuilderAdapter GetDummyMethod(TypeBuilderAdapter tb)
        {
			return dummy_methods[tb] as IMethodBuilderAdapter;
        }

		public void AddPascalTypeReference(ITypeNode tn, TypeAdapter t)
		{
			pas_defs[tn] = t;
		}
		
		public TypeAdapter GetPascalTypeReference(ITypeNode tn)
		{
			return pas_defs[tn] as TypeAdapter;
		}
		
		public ConstInfo AddConstant(IConstantDefinitionNode cnst, IFieldBuilderAdapter fb)
		{
			ConstInfo ci = new ConstInfo(fb);
			defs[cnst] = ci;
			return ci;
		}
		
        //добавление локальной переменной
		public VarInfo AddVariable(IVAriableDefinitionNode var, ILocalBuilderAdapter lb)
		{
			VarInfo vi = new VarInfo(lb);
			defs[var] = vi;
			return vi;
		}

        //ssyy
        public Label GetLabel(ILabelNode label, IILGeneratorAdapter il)
        {
            if (defs.ContainsKey(label))
            {
                return (Label)(defs[label]);
            }
            Label lab = il.DefineLabel();
            defs.Add(label, lab);
            return lab;
        }
        //\ssyy
		
        //получение локальной переменной
		public VarInfo GetVariable(IVAriableDefinitionNode var)
		{
			return (VarInfo)defs[var];
		}
		
        //добавление глоб. переменной
		public VarInfo AddGlobalVariable(IVAriableDefinitionNode var, IFieldBuilderAdapter fb)
		{
			VarInfo vi = new VarInfo();
			defs[var] = vi;
			vi.fb = fb;
			vi.kind = VarKind.vkGlobal;
			return vi;
		}
		
		public EvntInfo AddEvent(IEventNode ev, IEventBuilderAdapter eb)
		{
			EvntInfo ei = new EvntInfo(eb);
			defs[ev] = ei;
			return ei;
		}
		
		public EvntInfo GetEvent(IEventNode ev)
		{
			return (EvntInfo)defs[ev];
		}
		
        //добавление нелок. переменной
		public VarInfo AddNonLocalVariable(IVAriableDefinitionNode var, IFieldBuilderAdapter fb)
		{
			VarInfo vi = new VarInfo();
			defs[var] = vi;
			vi.fb = fb;
			vi.kind = VarKind.vkNonLocal;
			return vi;
		}
		
        //добавление функции (метода)
		public MethInfo AddMethod(IFunctionNode func, IMethodInfoAdapter mi)
		{
			MethInfo m = new MethInfo(mi);
			defs[func] = m;
			return m;
		}
		
        //добавление функции, вложенной в функцию
		public MethInfo AddMethod(IFunctionNode func, IMethodInfoAdapter mi, MethInfo up)
		{
			MethInfo m = new MethInfo(mi);
			m.up_meth = up;
			defs[func] = m;
			return m;
		}
		
        //получение метода
		public MethInfo GetMethod(IFunctionNode func)
		{
			return (MethInfo)defs[func];
		}
		
        //добавление конструктора
		public MethInfo AddConstructor(IFunctionNode func, IConstructorInfoAdapter ci)
		{
			//ConstrInfo m = new ConstrInfo(ci);
			MethInfo mi = new MethInfo();
			mi.cnstr = ci;
			defs[func] = mi;
			return mi;
		}
		
		public PropInfo AddProperty(IPropertyNode prop, IPropertyInfoAdapter pi)
		{
			PropInfo pi2 = new PropInfo(pi);
			defs[prop] = pi2;
			return pi2;
		}
		
		public PropInfo GetProperty(IPropertyNode prop)
		{
			return (PropInfo)defs[prop];
		}
		
        //получение конструктора
		public MethInfo GetConstructor(IFunctionNode func)
		{
			MethInfo mi = (MethInfo)defs[func];
			return mi;
		}
		
		public ConstInfo GetConstant(IConstantDefinitionNode cnst)
		{
			ConstInfo ci = (ConstInfo)defs[cnst];
			return ci;
		}

        public object GetConstantForExpression(IExpressionNode expr)
        {
            if (expr is PascalABCCompiler.TreeRealization.null_const_node) // SSM 20/04/21
                return expr;
            if (expr is IConstantNode)
                return (expr as IConstantNode).value;
            return null;
        }

        //добавление параметра
		public ParamInfo AddParameter(IParameterNode p, IParameterBuilderAdapter pb)
		{
			ParamInfo pi = new ParamInfo(pb);
			defs[p] = pi;
			return pi;
		}
		
        //добавление нелок. параметра
		public ParamInfo AddGlobalParameter(IParameterNode p, IFieldBuilderAdapter fb)
		{
			ParamInfo pi = new ParamInfo();
			pi.kind = ParamKind.pkGlobal;
			pi.fb = fb;
			defs[p] = pi;
			return pi;
		}
		
        //получение параметра
		public ParamInfo GetParameter(IParameterNode p)
		{
			return (ParamInfo)defs[p];
		}
		
        //добавление поля
		public FldInfo AddField(ICommonClassFieldNode f, IFieldInfoAdapter fb)
		{
			FldInfo fi = new FldInfo(fb);
#if DEBUG
            /*if (f.name == "XYZW")
            {
                var y = f.GetHashCode();
            } */
#endif
            defs[f] = fi;
            return fi;
		}
		
        public FldInfo AddGenericField(ICommonClassFieldNode f, IFieldInfoAdapter fb, TypeAdapter field_type, IFieldInfoAdapter prev_fi)
        {
            FldInfo fi = new GenericFldInfo(fb, field_type, prev_fi); // prev_fi - чтобы сконструировать на последнем этапе fi 
#if DEBUG
            /*if (f.name == "XYZW")
            {
                var y = f.GetHashCode();
            }*/
#endif
            defs[f] = fi;
            return fi;
        }

        //получение поля
		public FldInfo GetField(ICommonClassFieldNode f)
		{
            var r = (FldInfo)defs[f];
#if DEBUG
            /*if (f.name == "XYZW")
            {
                var y = f.GetHashCode();
            } */
#endif
#if DEBUG
            /*if (r == null && f.name == "XYZW")
            {
                foreach (var k in defs.Keys)
                {
                    if ((k is ICommonClassFieldNode) && (k as ICommonClassFieldNode).name == "XYZW")
                        return (FldInfo)defs[k];
                }
            } */
#endif
            return r;
		}
		
        //добавление типа
		public TypeInfo AddType(ITypeNode type, TypeBuilderAdapter tb)
		{
			TypeInfo ti = new TypeInfo(tb);
			defs[type] = ti;
			return ti;
		}
		
        public TypeInfo AddEnum(ITypeNode type, EnumBuilderAdapter emb)
        {
            TypeInfo ti = new TypeInfo(emb);
            defs[type] = ti;
            return ti;
        }

        public TypeInfo AddExistingType(ITypeNode type, TypeAdapter t)
        {
            TypeInfo ti = new TypeInfo(t);
            defs[type] = ti;
            return ti;
        }
		
        private IFunctionNode find_method(ICommonTypeNode tn, string name)
        {
        	foreach (ICommonMethodNode cmn in tn.methods)
        	{
        		if (string.Compare(cmn.name,name,true) == 0) return cmn;
        	}
            return null;
        }
        
        private IFunctionNode find_constructor(ICommonTypeNode tn)
        {
        	foreach (ICommonMethodNode cmn in tn.methods)
        	{
        		if (cmn.is_constructor) return cmn;
        	}
        	return null;
        }

        private IConstructorInfoAdapter find_constructor(TypeAdapter tn)
        {
            foreach (var cmn in tn.GetConstructors())
            {
                return cmn;
            }
            return null;
        }

        private IFunctionNode find_constructor_with_params(ICommonTypeNode tn)
        {
        	foreach (ICommonMethodNode cmn in tn.methods)
        	{
        		if (cmn.is_constructor && cmn.parameters.Length == 2) return cmn;
        	}
        	return null;
        }

        private IConstructorInfoAdapter find_constructor_with_params(TypeAdapter t)
        {
            foreach (IConstructorInfoAdapter ci in t.GetConstructors())
            {
                if (ci.GetParameters().Length == 2)
                    return ci;
            }
            return null;
        }

        private IFunctionNode find_constructor_with_one_param(ICommonTypeNode tn)
        {
        	foreach (ICommonMethodNode cmn in tn.methods)
        	{
        		if (cmn.is_constructor && cmn.parameters.Length == 1) return cmn;
        	}
        	return null;
        }

        private IConstructorInfoAdapter find_constructor_with_one_param(TypeAdapter t)
        {
            foreach (IConstructorInfoAdapter ci in t.GetConstructors())
            {
                if (ci.GetParameters().Length == 1)
                    return ci;
            }
            return null;
        }

        public bool IsConstructedGenericType(TypeAdapter t)
        {
            if (t is TypeBuilderAdapter || t is GenericTypeParameterBuilderAdapter || t is EnumBuilderAdapter || t.GetType().FullName == "System.Reflection.Emit.TypeBuilderInstantiation")
                return true;
            if (t.IsGenericType)
                foreach (TypeAdapter gt in t.GetGenericArguments())
                    if (IsConstructedGenericType(gt))
                        return true;
            if (t.IsArray)
                return IsConstructedGenericType(t.GetElementType());
            return false;
        }

        public bool IsNumericType(TypeAdapter t)
        {
            return t == TypeFactory.ByteType || t == TypeFactory.SByteType || t == TypeFactory.Int16Type || t == TypeFactory.UInt16Type
                || t == TypeFactory.Int32Type || t == TypeFactory.UInt32Type || t == TypeFactory.Int64Type || t == TypeFactory.UInt64Type
                || t == TypeFactory.SingleType || t == TypeFactory.DoubleType;
        }

        public ICommonTypeNode GetTypeNodeByITypeBuilderAdapter(TypeBuilderAdapter tb)
        {
            foreach (object o in defs.Keys)
            {
                if (o is ICommonTypeNode && this.GetTypeReference(o as ICommonTypeNode).tp == tb)
                    return o as ICommonTypeNode;
            }
            return null;
        }

        public IMethodInfoAdapter GetEnumeratorMethod(TypeAdapter t, out TypeAdapter[] generic_args)
        {
            generic_args = null;
            TypeAdapter generic_def = null;
            if (t.IsGenericType && !t.IsGenericTypeDefinition)
                generic_def = t.GetGenericTypeDefinition();
            else
                generic_def = t;
            if (generic_def.IsArray && generic_def.GetElementType().IsGenericParameter)
            {
                return TypeFactory.IEnumerableGenericType.GetMethod("GetEnumerator");
            }
            if (generic_def.IsArray)
            {
                if (IsConstructedGenericType(generic_def.GetElementType()))
                    return TypeBuilderAdapter.GetMethod(TypeFactory.IEnumerableGenericType.MakeGenericType(generic_def.GetElementType()), TypeFactory.IEnumerableGenericType.GetMethod("GetEnumerator"));
                else
                    return TypeFactory.IEnumerableGenericType.MakeGenericType(generic_def.GetElementType()).GetMethod("GetEnumerator");
            }
            else if (generic_def.IsGenericParameter)
            {
                return TypeFactory.IEnumerableType.GetMethod("GetEnumerator", TypeAdapter.EmptyTypes);
            }
            foreach (TypeAdapter interf in generic_def.GetInterfaces())
            {
                if (interf.IsGenericType && interf.GetGenericTypeDefinition() == TypeFactory.IEnumerableGenericType)
                {
	                IMethodInfoAdapter mi = interf.GetGenericTypeDefinition().GetMethod("GetEnumerator");
                    if (generic_def != t)
                    {
                        if (t.GetGenericArguments().Length != interf.GetGenericTypeDefinition().GetGenericArguments().Length)
                            return null;
                        TypeAdapter gt = interf.GetGenericTypeDefinition().MakeGenericType(t.GetGenericArguments());
                        if (IsConstructedGenericType(gt))
                            return TypeBuilderAdapter.GetMethod(gt, mi);
                        else
                            return interf.GetGenericTypeDefinition().MakeGenericType(t.GetGenericArguments()).GetMethod("GetEnumerator");
                    }
                    else if (IsConstructedGenericType(interf))
                    {
                        //return ITypeBuilderAdapter.GetMethod(TypeFactory.IEnumerableGenericType.MakeGenericType(interf.GetGenericArguments()), TypeFactory.IEnumerableGenericType.GetMethod("GetEnumerator"));
                        //return TypeFactory.IEnumerableType.GetMethod("GetEnumerator", TypeAdapter.EmptyTypes);
                        generic_args = interf.GetGenericArguments();
                        return TypeBuilderAdapter.GetMethod(interf, mi);
                    }
                    else
                        return interf.GetMethod("GetEnumerator");
                }
            }
            return TypeFactory.IEnumerableType.GetMethod("GetEnumerator", TypeAdapter.EmptyTypes);
        }

		public void SetAsProcessing(ICommonTypeNode type)
        {
            processing_types[type] = true;
        }

        public bool IsProcessing(ICommonTypeNode type)
        {
            return processing_types[type] != null;
        }

        public void LinkExpressionToLocalBuilder(IExpressionNode expr, ILocalBuilderAdapter lb)
        {
            memoized_exprs[expr] = lb;
        }

        public ILocalBuilderAdapter GetLocalBuilderForExpression(IExpressionNode expr)
        {
            return memoized_exprs[expr] as ILocalBuilderAdapter;
        }

        //получение типа
        public TypeInfo GetTypeReference(ITypeNode type)
		{
			TypeInfo ti = defs[type] as TypeInfo;
			if (ti != null) 
			{
				if (type.type_special_kind == type_special_kind.text_file) 
					ti.is_text_file = true;
				if (!ti.is_set && !ti.is_typed_file && !ti.is_text_file)
                    return ti;
				if (ti.clone_meth == null && !ti.is_typed_file && !ti.is_text_file)
                {
                    if (type is ICommonTypeNode)
                        ti.clone_meth = this.GetMethodBuilder(find_method(type as ICommonTypeNode, "CloneSet"));//ti.tp.GetMethod("Clone");
                    else
                        ti.clone_meth = ti.tp.GetMethod("CloneSet");
                }
                if (ti.def_cnstr == null)
                {
                	//if (type.type_special_kind == type_special_kind.text_file) ti.is_text_file = true;
                    if (ti.is_set)
                    {
                        if (type is ICommonTypeNode)
                            ti.def_cnstr = this.GetConstructorBuilder(find_constructor_with_params(type as ICommonTypeNode));
                        else
                            ti.def_cnstr = find_constructor_with_params(ti.tp);
                    }
                    else if (ti.is_typed_file)
                    {
                        if (type is ICommonTypeNode)
                            ti.def_cnstr = this.GetConstructorBuilder(find_constructor_with_one_param(type as ICommonTypeNode));
                        else
                            ti.def_cnstr = find_constructor_with_one_param(ti.tp);
                    }
                    else
                    {
                        if (type is ICommonTypeNode)
                            ti.def_cnstr = this.GetConstructorBuilder(find_constructor(type as ICommonTypeNode));
                        else
                            ti.def_cnstr = find_constructor(ti.tp);
                    }
                }
                if (ti.assign_meth == null && !ti.is_typed_file && !ti.is_text_file)
                {
                    if (type is ICommonTypeNode)
                        ti.assign_meth = this.GetMethodBuilder(find_method(type as ICommonTypeNode, "AssignSetFrom"));
                    else
                        ti.assign_meth = ti.tp.GetMethod("AssignSetFrom");
                }
				return ti;
			}
			if (type is ICompiledTypeNode) {
				ti = new TypeInfo(((ICompiledTypeNode)type).compiled_type);
				defs[type] = ti;
				return ti;
			}
            //(ssyy) Ускорил, вставив switch
            switch (type.type_special_kind)
            {
                case type_special_kind.typed_file:
                    ti = GetTypeReference(type.base_type);
                    if (ti == null) return null;
                    ti.is_typed_file = true;
                    if (ti.def_cnstr == null)
                    {
                        if (type.base_type is ICommonTypeNode)
                            ti.def_cnstr = this.GetConstructorBuilder(find_constructor_with_one_param(type.base_type as ICommonTypeNode));
                        else
                            ti.def_cnstr = find_constructor_with_one_param(ti.tp);
                    }
                    return ti;
                case type_special_kind.set_type:
                    ti = GetTypeReference(type.base_type);
                    if (ti == null) return null;
                    ti.is_set = true;
                    if (ti.clone_meth == null)
                    {
                        if (type.base_type is ICommonTypeNode)
                            ti.clone_meth = this.GetMethodBuilder(find_method(type.base_type as ICommonTypeNode, "CloneSet"));//ti.tp.GetMethod("Clone");
                        else
                            ti.clone_meth = ti.tp.GetMethod("CloneSet");
                    }
                    if (ti.assign_meth == null)
                    {
                        if (type.base_type is ICommonTypeNode)
                            ti.assign_meth = this.GetMethodBuilder(find_method(type.base_type as ICommonTypeNode, "AssignSetFrom"));
                        else
                            ti.assign_meth = ti.tp.GetMethod("AssignSetFrom");    
                    }
                    if (ti.def_cnstr == null)
                    {
                        if (type.base_type is ICommonTypeNode)
                            ti.def_cnstr = this.GetConstructorBuilder(find_constructor_with_params(type.base_type as ICommonTypeNode));
                        else
                            ti.def_cnstr = find_constructor_with_params(ti.tp);
                    }
                    return ti;
                case type_special_kind.diap_type:
                    return GetTypeReference(type.base_type);
                case type_special_kind.short_string:
                    return TypeFactory.string_type;
                case type_special_kind.array_kind:
                    TypeInfo tmp = GetTypeReference(type.element_type);
                    if (tmp == null) return null;
                    int rank = (type as ICommonTypeNode).rank;
                    if (rank == 1)
                    	ti = new TypeInfo(tmp.tp.MakeArrayType());
                    else
                    	ti = new TypeInfo(tmp.tp.MakeArrayType(rank));
                    //ti.is_arr = true;
                    defs[type] = ti;
                    return ti;
            }
			if (type is IRefTypeNode) {
				TypeInfo ref_ti = GetTypeReference(((IRefTypeNode)type).pointed_type);
                if (ref_ti == null) return null;
                //(ssyy) Лучше использовать MakePointerType
                ti = new TypeInfo(ref_ti.tp.MakePointerType());
                defs[type] = ti;
                return ti;
			}
			
			return null;
		}
		
		public IMethodBuilderAdapter GetMethodBuilder(IFunctionNode meth)
		{
			MethInfo mi = defs[meth] as MethInfo;
			if (mi != null)
			return mi.mi as IMethodBuilderAdapter;
			return null;
		}
		
		public IConstructorBuilderAdapter GetConstructorBuilder(IFunctionNode meth)
		{
			MethInfo ci = defs[meth] as MethInfo;
			if (ci != null)
			return ci.cnstr as IConstructorBuilderAdapter;
			return null;
		}
		
        //получение метода создания массива
		public IMethodInfoAdapter GetArrayInstance()
		{
			if (arr_mi != null) return arr_mi;
			arr_mi = AdapterFactory.Type(typeof(System.Array)).GetMethod("CreateInstance",new TypeAdapter[2]{typeof(Type).GetAdapter(), typeof(int).GetAdapter()});
			return arr_mi;
		}

        //добавление фиктивного метода (если метод содерж. вложенные, создается заглушка)
        //т. е. метод не добавл. в таблицу
        public MethInfo AddFictiveMethod(IFunctionNode func, IMethodBuilderAdapter mi)
        {
            MethInfo m = new MethInfo(mi);
            //defs[func] = m;
            return m;
        }
    }
	
	
}
