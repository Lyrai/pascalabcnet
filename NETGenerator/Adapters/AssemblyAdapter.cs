namespace PascalABCCompiler.NETGenerator.Adapters
{
    public abstract class AssemblyAdapter
    {
        public abstract TypeAdapter[] GetTypes();
        public abstract TypeAdapter GetType(string name, bool throwOnError);

        public static AssemblyAdapter Load(byte[] bytes)
        {
            return null;
        }

        public static AssemblyAdapter LoadFrom(string filename)
        {
            return null;
        }
    }
}