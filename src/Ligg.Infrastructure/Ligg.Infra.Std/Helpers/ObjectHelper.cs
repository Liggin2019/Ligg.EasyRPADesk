using System;
using System.Data;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using Newtonsoft.Json;


namespace Ligg.Infrastructure.Helpers
{
    public static partial class ObjectHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //*convert
        //*judge
        public static bool IsType(Type type, string typeName)
        {
            try
            {
                if (type.ToString() == typeName) return true;
                if (type.ToString() == "System.Object") return false;
                return IsType(type.BaseType, typeName);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".IsType Error: " + ex.Message);
            }
        }


    }
}
