using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Roslyn.Utilities;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynTypeAdapter: ITypeAdapter
    {
        public bool IsGenericType { get; }
        public bool IsArray { get; }
        public bool IsGenericTypeDefinition { get; }
        public bool IsGenericParameter { get; }
        public bool IsValueType { get; }
        public bool IsPointer { get; }
        public bool IsEnum { get; }
        public bool IsInterface { get; }
        public bool IsClass { get; }
        public bool IsPrimitive { get; }
        public bool IsSealed { get; }
        public bool IsAbstract { get; }
        public bool IsByRef { get; }
        public bool IsNotPublic { get; }
        public bool IsPublic { get; }
        public IMethodInfoAdapter DeclaringMethod { get; }
        public string FullName { get; }
        public string Name { get; }
        public string Namespace { get; }
        public string AssemblyQualifiedName { get; }
        public AssemblyAdapter Assembly { get; }
        public int GenericParameterPosition { get; }
        public ITypeAdapter BaseType { get; protected set; }
        public ITypeAdapter DeclaringType { get; }
        public IModuleAdapter Module { get; }

        protected Dictionary<string, List<IMemberInfoAdapter>> _members = new Dictionary<string, List<IMemberInfoAdapter>>();
        protected Dictionary<string, ITypeAdapter> _netstedTypes = new Dictionary<string, ITypeAdapter>();
        private List<ITypeAdapter> _interfaces;

        public RoslynTypeAdapter(IModuleAdapter module, string name, TypeAttributes attr, ITypeAdapter parent, ITypeAdapter[] interfaces)
        {
            FullName = name;
            Name = name.Split('.').Last();
            Namespace = name.Remove(name.LastIndexOf('.'));
            _interfaces = interfaces is object ? new List<ITypeAdapter>(interfaces) : new List<ITypeAdapter>();
            IsAbstract = (attr & TypeAttributes.Abstract) != 0;
            IsInterface = (attr & TypeAttributes.Interface) != 0;
            IsPublic = (attr & TypeAttributes.Public) != 0;
            IsNotPublic = !IsPublic;
            BaseType = parent ?? typeof(object).GetAdapter();
            Module = module;
            Assembly = module.Assembly;
        }

        protected RoslynTypeAdapter(ITypeBuilderAdapter declaringType, string name, TypeAttributes attr)
            : this(declaringType.Module, "", attr, null, null)
        {
            Name = name;
            DeclaringType = declaringType;
            FullName = declaringType.FullName + "+" + name;
            Namespace = "";
        }

        public IMethodInfoAdapter GetMethod(string name)
        {
            if (!_members.ContainsKey(name))
            {
                return null;
            }
            
            return _members[name].First() as IMethodInfoAdapter;
        }

        public IMethodInfoAdapter GetMethod(string name, ITypeAdapter[] parameterTypes)
        {
            if (!_members.ContainsKey(name))
            {
                return null;
            }

            return FindMethodBySignature(name, parameterTypes);
        }

        public IMethodInfoAdapter GetMethod(string name, BindingFlags flags)
        {
            if (!_members.ContainsKey(name))
            {
                return null;
            }

            foreach (var method in _members[name].Where(member => member is IMethodInfoAdapter).Cast<IMethodInfoAdapter>())
            {
                if ((flags & BindingFlags.Public) != 0 && !method.IsPublic)
                {
                    continue;
                }

                if ((flags & BindingFlags.Static) != 0 && !method.IsStatic)
                {
                    continue;
                }

                if ((flags & BindingFlags.NonPublic) != 0 && method.IsPublic)
                {
                    continue;
                }

                return method;
            }


            return null;
        }

        public IMethodInfoAdapter[] GetMethods()
        {
            return _members
                .Values
                .Flatten()
                .Where(member => member is IMethodInfoAdapter)
                .Cast<IMethodInfoAdapter>()
                .ToArray();
        }

        public IMethodInfoAdapter[] GetMethods(BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public IConstructorInfoAdapter GetConstructor(ITypeAdapter[] parameterTypes)
        {
            throw new System.NotImplementedException();
        }

        public IConstructorInfoAdapter[] GetConstructors()
        {
            throw new System.NotImplementedException();
        }

        public IConstructorInfoAdapter[] GetConstructors(BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter[] GetGenericArguments()
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter GetElementType()
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter GetGenericTypeDefinition()
        {
            throw new System.NotImplementedException();
        }

        public IPropertyInfoAdapter GetProperty(string name)
        {
            throw new System.NotImplementedException();
        }

        public IPropertyInfoAdapter GetProperty(string name, BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public IFieldInfoAdapter GetField(string name, BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter GetInterface(string name)
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter[] GetInterfaces()
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter[] GetNestedTypes()
        {
            throw new System.NotImplementedException();
        }

        public IMemberInfoAdapter[] GetMember(string name, BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public IFieldInfoAdapter[] GetFields()
        {
            throw new System.NotImplementedException();
        }

        public IMemberInfoAdapter[] GetDefaultMembers()
        {
            throw new System.NotImplementedException();
        }

        public IMemberInfoAdapter[] GetMembers(BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public int GetArrayRank()
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter MakeGenericType(ITypeAdapter type)
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter MakeGenericType(ITypeAdapter[] types)
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter MakeArrayType()
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter MakeArrayType(int rank)
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter MakePointerType()
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter MakeByRefType()
        {
            throw new System.NotImplementedException();
        }

        public object[] GetCustomAttributes(ITypeAdapter attributeType, bool inherit)
        {
            throw new System.NotImplementedException();
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            throw new System.NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RoslynTypeAdapter adapter))
            {
                return false;
            }

            return adapter.FullName == FullName;
        }

        protected IMethodInfoAdapter FindMethodBySignature(string name, ITypeAdapter[] parameterTypes)
        {
            if (!_members.ContainsKey(name))
            {
                return null;
            }
            
            if (parameterTypes is null)
            {
                parameterTypes = new ITypeAdapter[] { };
            }
            
            return _members[name]
                .Where(meth => meth is IMethodInfoAdapter)
                .Cast<IMethodInfoAdapter>()
                .First(
                    meth => meth
                        .GetParameters()
                        .Select(param => param.ParameterType)
                        .SequenceEqual(parameterTypes)
                );
        }

        protected IConstructorInfoAdapter FindConstructorBySignature(ITypeAdapter[] parameterTypes)
        {
            if (!_members.ContainsKey(".ctor"))
            {
                return null;
            }

            if (parameterTypes is null)
            {
                parameterTypes = new ITypeAdapter[] { };
            }
            
            return _members[".ctor"]
                .Where(meth => meth is IConstructorInfoAdapter)
                .Cast<IConstructorInfoAdapter>()
                .First(
                    meth => meth
                        .GetParameters()
                        .Select(param => param.ParameterType)
                        .SequenceEqual(parameterTypes)
                );
        }
    }
}