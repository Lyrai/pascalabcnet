using System;
using System.Reflection;
using System.Reflection.Emit;

namespace PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters
{
    internal class RoslynAssemblyAdapter: AssemblyAdapter
    {
        protected RoslynModuleBuilderAdapter _moduleBuilder;
        
        public override ITypeAdapter[] GetTypes()
        {
            return _moduleBuilder.GetTypes();
        }

        public override ITypeAdapter GetType(string name, bool throwOnError)
        {
            var type = _moduleBuilder.GetType(name);
            if (type is null && throwOnError)
            {
                throw new TypeLoadException();
            }

            return type;
        }
    }
}