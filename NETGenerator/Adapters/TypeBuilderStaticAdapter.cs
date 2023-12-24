using System.Reflection.Emit;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public static class TypeBuilderStaticAdapter
    {
        public static IMethodInfoAdapter GetMethod(ITypeAdapter type, IMethodInfoAdapter method)
        {
#if NET472
            return TypeBuilder.GetMethod((type as FrameworkTypeAdapter).Adaptee, (method as FrameworkMethodInfoAdapter).Adaptee).GetAdapter();
#else
            return null;
#endif
        }

        public static IConstructorInfoAdapter GetConstructor(ITypeAdapter type, IConstructorInfoAdapter constructor)
        {
#if NET472
            return TypeBuilder.GetConstructor((type as FrameworkTypeAdapter).Adaptee, (constructor as FrameworkConstructorInfoAdapter).Adaptee).GetAdapter();
#else
            return null;
#endif
        }

        public static IFieldInfoAdapter GetField(ITypeAdapter type, IFieldInfoAdapter field)
        {
#if NET472
            return TypeBuilder.GetField((type as FrameworkTypeAdapter).Adaptee, (field as FrameworkFieldInfoAdapter).Adaptee).GetAdapter();
#else
            return null;
#endif
        }
    }
}