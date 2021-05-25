using System;
using System.Collections.Generic;

namespace Ligg.Infrastructure.Utility.Ioc
{
    public interface IDependencyResolver : IDisposable
    {
        void SetContainer();
        void Register(Type type);


        void Register(string assemblyPath, string TypeName);
        void RegisterAssemblyTypesByPath(string assemblyPath);

        void RegisterAssemblyTypesByName(string assemblyName);
        void Register<T>(T instance);

        void Inject<T>(T existing);

        object Resolve(string assemblyPath, string TypeName);

        object Resolve(Type type);

        T Resolve<T>(Type type);

        T Resolve<T>(Type type, string name);

        T Resolve<T>();

        T Resolve<T>(string name);

        IEnumerable<T> ResolveAll<T>();

        object[] ResolveAll(string assemblyPath, string[] TypeNames);

    }

}
