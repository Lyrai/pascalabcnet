using System;
using System.Reflection;

namespace NETGenerator.Adapters
{
    public abstract class TypeBuilderAdapter: TypeAdapter
    {
        public static MethodInfo GetMethod(TypeAdapter type, MethodInfo method)
        {
            return null;
        }
    }
}