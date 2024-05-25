using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Roslyn.Utilities;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynTypeAdapter: ITypeAdapter
    {
        public virtual bool IsGenericType { get; }
        public virtual bool IsArray { get; protected set; }
        public virtual bool IsGenericTypeDefinition { get; }
        public virtual bool IsGenericParameter { get; }
        public bool IsValueType { get; }
        public bool IsPointer { get; }
        public bool IsEnum { get; }
        public bool IsInterface { get; }
        public bool IsClass { get; }
        public bool IsPrimitive { get; }
        public bool IsSealed { get; }
        public bool IsAbstract { get; }
        public bool IsByRef => Name.EndsWith("&");
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
        public TypeAttributes Attributes { get; }
        public virtual IEnumerable<ITypeAdapter> ImplementedInterfaces => _interfaces;

        protected Dictionary<string, List<IMemberInfoAdapter>> _members = new Dictionary<string, List<IMemberInfoAdapter>>();
        protected Dictionary<string, ITypeAdapter> _netstedTypes = new Dictionary<string, ITypeAdapter>();
        protected List<ITypeAdapter> _interfaces;

        protected RoslynTypeAdapter(IModuleAdapter module, string name, TypeAttributes attr, ITypeAdapter parent, ITypeAdapter[] interfaces)
        {
            if (name.Contains('`'))
            {
                name = name.Remove(name.IndexOf('`'));
            }
            FullName = name;
            Name = name.Split('.').Last();
            Namespace = name.Contains('.') ? name.Remove(name.LastIndexOf('.')) : "";
            _interfaces = interfaces is object ? new List<ITypeAdapter>(interfaces) : new List<ITypeAdapter>();
            IsAbstract = (attr & TypeAttributes.Abstract) != 0;
            IsInterface = (attr & TypeAttributes.Interface) != 0;
            IsPublic = (attr & TypeAttributes.Public) != 0;
            IsNotPublic = !IsPublic;
            BaseType = IsInterface ? parent : parent ?? typeof(object).GetAdapter();
            Module = module;
            Assembly = module.Assembly;
            IsClass = true;
            Attributes = attr;
        }

        protected RoslynTypeAdapter(ITypeBuilderAdapter declaringType, string name, TypeAttributes attr)
            : this(declaringType.Module, "", attr, null, null)
        {
            Name = name;
            DeclaringType = declaringType;
            FullName = declaringType.FullName + "+" + name;
            Namespace = "";
        }

        public virtual IMethodInfoAdapter GetMethod(string name)
        {
            if (!_members.ContainsKey(name))
            {
                return null;
            }
            
            return _members[name].First() as IMethodInfoAdapter;
        }

        public virtual IMethodInfoAdapter GetMethod(string name, ITypeAdapter[] parameterTypes)
        {
            if (!_members.ContainsKey(name))
            {
                return null;
            }

            return FindMethodBySignature(name, parameterTypes);
        }

        public virtual IMethodInfoAdapter GetMethod(string name, BindingFlags flags)
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

        public virtual IMethodInfoAdapter[] GetMethods()
        {
            return _members
                .Values
                .Flatten()
                .OfType<IMethodInfoAdapter>()
                .ToArray();
        }

        public virtual IPropertyInfoAdapter[] GetProperties()
        {
            return _members
                .Values
                .Flatten()
                .OfType<IPropertyInfoAdapter>()
                .ToArray();
        }

        public virtual IMethodInfoAdapter[] GetMethods(BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public virtual IConstructorInfoAdapter GetConstructor(ITypeAdapter[] parameterTypes)
        {
            if (!_members.ContainsKey(".ctor") && !_members.ContainsKey(".cctor"))
            {
                return null;
            }
            
            return FindConstructorBySignature(parameterTypes);
        }

        public virtual IConstructorInfoAdapter[] GetConstructors()
        {
            return _members
                .Values
                .Flatten()
                .OfType<IConstructorInfoAdapter>()
                .ToArray();
        }

        public IConstructorInfoAdapter[] GetConstructors(BindingFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public virtual ITypeAdapter[] GetGenericArguments()
        {
            throw new System.NotImplementedException();
        }

        public virtual ITypeAdapter GetElementType()
        {
            throw new NotSupportedException();
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

        public virtual IFieldInfoAdapter GetField(string name, BindingFlags flags)
        {
            if (!_members.ContainsKey(name))
            {
                return null;
            }

            foreach (var field in _members[name].Where(member => member is IFieldInfoAdapter).Cast<IFieldInfoAdapter>())
            {
                if ((flags & BindingFlags.Public) != 0 && !field.IsPublic)
                {
                    continue;
                }

                if ((flags & BindingFlags.Static) != 0 && !field.IsStatic)
                {
                    continue;
                }

                if ((flags & BindingFlags.NonPublic) != 0 && field.IsPublic)
                {
                    continue;
                }

                return field;
            }


            return null;
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
            return _members
                .Values
                .Flatten()
                .OfType<IFieldInfoAdapter>()
                .ToArray();
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

        public virtual ITypeAdapter MakeGenericType(params ITypeAdapter[] types)
        {
            throw new System.NotImplementedException();
        }

        public virtual ITypeAdapter MakeArrayType()
        {
            return new RoslynArrayTypeAdapter(this);
        }

        public ITypeAdapter MakeArrayType(int rank)
        {
            throw new System.NotImplementedException();
        }

        public ITypeAdapter MakePointerType()
        {
            throw new System.NotImplementedException();
        }

        public virtual ITypeAdapter MakeByRefType()
        {
            throw new NotSupportedException();
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
                .FirstOrDefault(
                    meth => meth
                        .GetParameters()
                        .Select(param => param.ParameterType)
                        .SequenceEqual(parameterTypes)
                );
        }

        protected IConstructorInfoAdapter FindConstructorBySignature(ITypeAdapter[] parameterTypes)
        {
            parameterTypes = parameterTypes ?? new ITypeAdapter[] { };

            IConstructorInfoAdapter result = null;
            
            if (_members.TryGetValue(".ctor", out var instanceConstructors))
            {
                result = instanceConstructors
                    .Where(meth => meth is IConstructorInfoAdapter)
                    .Cast<IConstructorInfoAdapter>()
                    .FirstOrDefault(
                        meth => meth
                            .GetParameters()
                            .Select(param => param.ParameterType)
                            .SequenceEqual(parameterTypes)
                    );
            }

            if (result is object)
            {
                return result;
            }
            
            if (_members.TryGetValue(".cctor", out var staticConstructors))
            {
                result = staticConstructors
                    .Where(meth => meth is IConstructorInfoAdapter)
                    .Cast<IConstructorInfoAdapter>()
                    .FirstOrDefault(
                        meth => meth
                            .GetParameters()
                            .Select(param => param.ParameterType)
                            .SequenceEqual(parameterTypes)
                    );
            }

            return result;
        }
    }
}