using System;
using System.Reflection.Emit;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public static class TypeStaticAdapter
    {
        public static ITypeAdapter[] EmptyTypes => new ITypeAdapter[] { };
        
        public static TypeCode GetTypeCode(ITypeAdapter type)
        {
#if NET472
            return Type.GetTypeCode((type as FrameworkTypeAdapter)?.Adaptee);
#else
            return TypeCode.Boolean;
#endif
        }

        public static ITypeAdapter GetType(string name)
        {
            return Type.GetType(name).GetAdapter();
        }
    }
}