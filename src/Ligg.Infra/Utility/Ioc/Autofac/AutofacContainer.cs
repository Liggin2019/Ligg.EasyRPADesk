using Autofac;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Utility.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ligg.Infrastructure.Utility.Ioc
{
    //public class AutofacContainer : DisposableResource, IContainer
    public class AutofacContainer : IDependencyResolver
    {
        private readonly ContainerBuilder _containerBuilder;
        private IContainer _container;
        public AutofacContainer()
        {
            _containerBuilder = new ContainerBuilder();
        }
        public void SetContainer()
        {
            _container = _containerBuilder.Build();
        }

        //# Register
        public void Register(Type type)
        {
            _containerBuilder.RegisterType(type);
        }

        public void Register(string assemblyPath, string TypeName)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            var type = assembly.GetType(TypeName);
            _containerBuilder.RegisterType(type);
        }

        public void RegisterAssemblyTypesByPath(string assemblyPath)
        {
            _containerBuilder.RegisterAssemblyTypes(Assembly.LoadFrom(assemblyPath));
        }

        public void RegisterAssemblyTypesByName(string assemblyName)
        {
            _containerBuilder.RegisterAssemblyTypes(Assembly.Load(assemblyName));
        }
        public void Register<T>(T instance)
        {
            throw new NotImplementedException();
        }

        //# Inject
        public void Inject<T>(T existing)
        {
            throw new NotImplementedException();
        }

        //# Resolve
        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }
        public T Resolve<T>(Type type)
        {
            return (T)_container.Resolve(type);
        }

        public object Resolve(string assemblyPath, string TypeName)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            var type = assembly.GetType(TypeName);
            return _container.Resolve(type);
        }

        public T Resolve<T>(Type type, string name)
        {
            return (T)_container.ResolveNamed<T>(name);
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public T Resolve<T>(string name)
        {
            return _container.ResolveNamed<T>(name);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return _container.Resolve<IEnumerable<T>>();
        }

        public object[] ResolveAll(string assemblyPath, string[] TypeNames)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            var types = new Type[TypeNames.Length];
            var objs= new object[TypeNames.Length];
            for (int i = 0; i < TypeNames.Length; i++)
            {
                var type = assembly.GetType(TypeNames[i]);
                types[i] = type;
                objs[i]=_container.Resolve(type);
            }
            return objs;
        }

        //**Special
        public void SetGlobalConfigurationResolver()
        {
            //var configuration = GlobalConfiguration.Configuration;
            //var resolver = new AutofacWebApiDependencyResolver(_container);
            //configuration.DependencyResolver = resolver;
        }

        //**Common
        public void Dispose()
        {
            _container.Dispose();

            //base.Dispose(disposing);
        }

    }
}
