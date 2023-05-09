using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;


namespace Ligg.Infrastructure.Helpers
{
    public static class GenericHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;


        //*convert
        public static string ConvertToJson<T>(T gnrc, string dateTimeFormat = "")
        {
            if (gnrc == null) return null;
            var json = JsonHelper.Serialize(gnrc, dateTimeFormat);
            return json;
        }

    }
}
