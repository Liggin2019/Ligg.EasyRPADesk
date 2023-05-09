using System;
using System.Data;
using System.Reflection;
using Ligg.Infrastructure.Extensions;
using Newtonsoft.Json;


namespace Ligg.Infrastructure.Helpers
{

    internal static partial class ObjectHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;


        internal static string ParseToString(object obj)
        {
            try
            {
                if (obj == null)
                {
                    return string.Empty;
                }
                else
                {
                    return obj.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }
        }



    }

}
