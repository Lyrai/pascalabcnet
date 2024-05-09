using System;
using PascalABCCompiler.NETGenerator.Adapters.NetFrameworkAdapters;

namespace PascalABCCompiler.NETGenerator.Adapters
{
    public class EnumAdapter: IAdapter
    {
        public static object ToObject(ITypeAdapter type, int value)
        {
#if !NETCOREAPP
            return Enum.ToObject((type as FrameworkTypeAdapter).Adaptee, value);
#else
            return null;
#endif
        }
        
        public static object ToObject(ITypeAdapter type, object value)
        {
#if !NETCOREAPP
            return Enum.ToObject((type as FrameworkTypeAdapter).Adaptee, value);
#else
            return null;
#endif
        }
    }
}