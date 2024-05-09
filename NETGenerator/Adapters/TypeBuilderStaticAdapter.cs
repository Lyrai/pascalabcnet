using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;
using PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public static class TypeBuilderStaticAdapter
    {
        public static IMethodInfoAdapter GetMethod(ITypeAdapter type, IMethodInfoAdapter method)
        {
#if !NETCOREAPP
            return TypeBuilder.GetMethod((type as FrameworkTypeAdapter).Adaptee, (method as FrameworkMethodInfoAdapter).Adaptee).GetAdapter();
#else
            if (type is RoslynGenericTypeAdapter native)
            {
                return TypeBuilder.GetMethod(native.ConstructedFrom, (method as FrameworkMethodInfoAdapter).Adaptee).GetAdapter();
            }
            
            if (type is RoslynTypeBuilderAdapter source)
            {
                return source.GetMethod(method.Name, method.GetParameters().Select(param => param.ParameterType).ToArray());
            }

            return null;
#endif
        }

        public static IConstructorInfoAdapter GetConstructor(ITypeAdapter type, IConstructorInfoAdapter constructor)
        {
#if !NETCOREAPP
            return TypeBuilder.GetConstructor((type as FrameworkTypeAdapter).Adaptee, (constructor as FrameworkConstructorInfoAdapter).Adaptee).GetAdapter();
#else
            if (type is RoslynGenericTypeAdapter native)
            {
                return TypeBuilder.GetConstructor(native.ConstructedFrom, (constructor as FrameworkConstructorInfoAdapter).Adaptee).GetAdapter();
            }
            
            if (type is RoslynTypeBuilderAdapter source)
            {
                return source.GetConstructor(constructor.GetParameters().Select(param => param.ParameterType).ToArray());
            }
            return null;
#endif
        }

        public static IFieldInfoAdapter GetField(ITypeAdapter type, IFieldInfoAdapter field)
        {
#if !NETCOREAPP
            return TypeBuilder.GetField((type as FrameworkTypeAdapter).Adaptee, (field as FrameworkFieldInfoAdapter).Adaptee).GetAdapter();
#else
            Console.WriteLine("TypeBuilderStaticAdapter.GetField not implemented");
            return null;
#endif
        }
    }
}