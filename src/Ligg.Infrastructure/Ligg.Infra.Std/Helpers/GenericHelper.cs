using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using Ligg.Infrastructure.Extensions;


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


        //##RichText
        public static string ConvertListToRichText<T>(List<T> list, bool ignoreFieldNames = false, string[] excludedfieldArray = null)
        {
            if (excludedfieldArray != null) excludedfieldArray = excludedfieldArray.ToLower();
            if (list == null) return "";
            if (list.Count == 0) return "";

            var strBlder = new StringBuilder();

            if (list.Count > 0)
            {
                Type tp = typeof(T);
                var fields = tp.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (!ignoreFieldNames)
                {
                    var txt = "";
                    var ct = 0;
                    foreach (var field in fields)
                    {
                        var name = field.Name.GetSubStringSurroundedByTwoDifferentIdentifiers("<", ">");
                        if (!excludedfieldArray.Contains(name.ToLower()))
                        {
                            txt = ct == 0 ? name : txt + "\t" + name;
                        }

                        ct++;
                    }
                    strBlder.AppendLine(txt);
                }


                for (int i = 0; i < list.Count; i++)
                {
                    var ct = 0;
                    var txt1 = "";
                    foreach (var field in fields)
                    {
                        var name = field.Name.GetSubStringSurroundedByTwoDifferentIdentifiers("<", ">");
                        if (!excludedfieldArray.Contains(name.ToLower()))
                        {
                            object obj = field.GetValue(list[i]);
                            txt1 = ct == 0 ? "" + obj : txt1 + "\t" + obj;
                        }


                        ct++;
                    }
                    strBlder.AppendLine(txt1);
                }
            }

            return strBlder.ToString();
        }



    }
}
