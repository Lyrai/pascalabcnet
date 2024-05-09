using System;
using System.Reflection.Emit;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;
using PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public static class TypeStaticAdapter
    {
        public static ITypeAdapter[] EmptyTypes => new ITypeAdapter[] { };
        
        public static TypeCode GetTypeCode(ITypeAdapter type)
        {
#if !NETCOREAPP
            return Type.GetTypeCode((type as FrameworkTypeAdapter)?.Adaptee);
#else
            if (type is FrameworkTypeAdapter framework)
            {
                return Type.GetTypeCode(framework.Adaptee);
            }

            return TypeCode.Object;
#endif
        }

        public static ITypeAdapter GetType(string name)
        {
            return Type.GetType(name).GetAdapter();
        }
    }
}