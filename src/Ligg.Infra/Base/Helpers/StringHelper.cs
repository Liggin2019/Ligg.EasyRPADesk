using System;
using System.Collections.Generic;
using System.Text;
using Ligg.Infrastructure.Base.Extension;

namespace Ligg.Infrastructure.Base.Helpers
{
    public static class StringHelper
    {

        public static string[] WashArray(string[] arry)
        {
            if (arry.Length == 0 | arry == null) return arry;
            arry = TrimArray(arry);
            arry = ClearArray(arry);
            return arry;
        }

        public static string[] TrimArray(string[] arry)
        {
            if (arry.Length == 0 | arry == null) return arry;
            var arry1 = new string[arry.Length];
            for (int i = 0; i < arry.Length; i++)
            {
                arry1[i] = arry[i].Trim();
            }
            return arry1;
        }

        public static string[] ClearArray(string[] arry)
        {
            if (arry.Length == 0 | arry == null) return arry;
            var arry1 = new string[arry.Length];
            var list = new List<string>();
            for (int i = 0; i < arry.Length; i++)
            {
                if (!arry[i].IsNullOrEmpty())
                {
                    list.Add(arry[i]);
                }
            }
            return list.ToArray();
        }

        public static string UnwrapStringArrayBySeparator(String[] strArry, char separator)
        {
            string str = "";
            if (strArry.Length == 0 | strArry == null) return string.Empty;
            for (int i = 0; i < strArry.Length; i++)
            {
                if (i == 0)
                {
                    str = strArry[i];
                }
                else
                {
                    str = str + separator + strArry[i];
                }
            }
            return str;
        }

        public static string ConvertIntegerListToIdsString<T>(IEnumerable<T> integerList, char separator)
        {
            string ids = "";
            if (integerList != null)
            {
                int i = 0;
                foreach (var v in integerList)
                {
                    if (i == 0)
                    {
                        ids = v.ToString();
                    }
                    else
                    {
                        ids = ids + separator + v.ToString();
                    }
                    i++;
                }
            }
            return ids;
        }

    }
}
