using System;
using System.Collections.Generic;
using Ligg.Infrastructure.Base;


namespace Ligg.Infrastructure.Utility.Ioc
{
    //# IoC操作类, 使用该类，可以使程序支持多种IoC框架, 例如： Ioc.InitializeWith("namespace.AutofacContainer"); Ioc.Resolve;
    public static class Ioc
    {
        private static IDependencyResolver _resolver;

        public static void InitializeWith(IDependencyResolverFactory factory)
        {
            Check.Argument.IsNotNull(factory, "factory");
            _resolver = factory.CreateInstance();
        }

        public static void SetContainer()
        {
            _resolver.SetContainer();
        }

        public static void RegisterType(Type type)
        {
            _resolver.Register(type);
        }

        public static void RegisterAssemblyTypesByPath(string assemblyPath)
        {
            _resolver.RegisterAssemblyTypesByPath(assemblyPath);
        }

        public static void Register(string assemblyPath,string TypeName)
        {
            _resolver.Register(assemblyPath, TypeName);
        }

        public static void Register<T>(T instance)
        {
            _resolver.Register(instance);
        }

        public static void Inject<T>(T existing)
        {
            _resolver.Inject(existing);
        }

        public static object Resolve(string assemblyPath, string TypeName)
        {
            return _resolver.Resolve(assemblyPath,  TypeName);
        }
        public static T Resolve<T>(Type type)
        {
            return _resolver.Resolve<T>(type);
        }
        public static T Resolve<T>(Type type, string name)
        {
            return _resolver.Resolve<T>(type, name);
        }
        public static T Resolve<T>()
        {
            return _resolver.Resolve<T>();
        }
        public static T Resolve<T>(string name)
        {
            return _resolver.Resolve<T>(name);
        }
        public static IEnumerable<T> ResolveAll<T>()
        {
            return _resolver.ResolveAll<T>();
        }
        public static object[] ResolveAll(string assemblyPath, string[] TypeNames)
        {
            return _resolver.ResolveAll(assemblyPath, TypeNames);
        }

        public static void Reset()
        {
            if (_resolver != null)
            {
                _resolver.Dispose();
            }
        }


    }

}
