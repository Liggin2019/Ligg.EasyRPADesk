using System;
using System.Configuration;

namespace Ligg.Infrastructure.Utility.Ioc
{
    public class DependencyResolverFactory : IDependencyResolverFactory
    {
        private Type _resolverType;

        public DependencyResolverFactory() : this(ConfigurationManager.AppSettings["dependencyResolverTypeName"])
        {
        }

        public DependencyResolverFactory(string resolverTypeName)
        {
            _resolverType = Type.GetType(resolverTypeName, true, true);
        }

        public IDependencyResolver CreateInstance()
        {
            return Activator.CreateInstance(_resolverType) as IDependencyResolver;
            //return new AutofacContainer();
        }
    }
}
