using System;

namespace NETGenerator.Adapters
{
    public abstract class AdapterFactory
    {
        private static AdapterFactory _instance = null;
        
        public static IAppDomainAdapter AppDomain()
        {
            return Instance().CreateAppDomain();
        }

        public static TypeAdapter Type(Type type)
        {
            return Instance().CreateType(type);
        }

        protected abstract IAppDomainAdapter CreateAppDomain();
        protected abstract TypeAdapter CreateType(Type type);

        private static AdapterFactory Instance()
        {
            return _instance ?? null;
        }
    }
}