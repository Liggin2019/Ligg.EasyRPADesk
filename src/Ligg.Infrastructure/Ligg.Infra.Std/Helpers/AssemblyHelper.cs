using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ligg.Infrastructure.Helpers
{
    public class AssemblyHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static object CreateObject(string assemblyPath, string namespaceDotClassName)
        {
            object objType = null;
            try
            {
                //objType = Assembly.Load(assName).CreateInstance(namespaceDotClassName);//only valid for that in same folder as main exe
                objType = Assembly.LoadFrom(assemblyPath).CreateInstance(namespaceDotClassName);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".CreateObject Error: " + ex.Message);
            }
            return objType;
        }

        private static ObjectDictionary<string, object> _objectDict= new ObjectDictionary<string, object>();

        public static void SetCache(string key, object value)
        {
            lock (_objectDict.LockObj)
            {
                _objectDict[key] = value;
            }
        }

        public static object GetCache(string key)
        {
            lock (_objectDict.LockObj)
            {
                return _objectDict.ContainsKey(key) ? _objectDict[key] : null;
            }
        }
    }

    internal class ObjectDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        internal object LockObj = new object();
    }
}
