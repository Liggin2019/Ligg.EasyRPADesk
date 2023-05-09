using System;
using System.Collections.Generic;
using Ligg.Infrastructure.Extensions;

namespace Ligg.Infrastructure.Extensions
{
    public static class StringArrayExtension
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        public static string[] Extract(this string[] arry, int index, int index1)
        {
            if (arry.Length == 0 | arry == null) return null;
            if (index < 0) throw new ArgumentException(_typeFullName + ".Extract Error: " + "index can't be less than 0!");
            if (index1 < 0) throw new ArgumentException(_typeFullName + ".Extract Error: " + "index1 can't be less than 0!");
            if (index > arry.Length - 1) throw new ArgumentException(_typeFullName + ".Extract Error: " + "index: " + index + "can't be greater than " + (arry.Length - 1).ToString() + "!");
            if (index1 > arry.Length - 1) throw new ArgumentException(_typeFullName + ".Extract Error: " + "index1: " + index1 + " can't be greater than" + (arry.Length - 1).ToString() + "!");

            var count = Math.Abs(index1 - index + 1);
            var arry1 = new string[count];
            var i = 0;
            if (index1 > index)
            {
                for (int n = index; n < index1 + 1; n++)
                {
                    arry1[i] = arry[n];
                    i++;
                }
            }
            else
            {
                for (int n = index1; n > index - 1; n--)
                {
                    arry1[i] = arry[n];
                    i++;
                }
            }

            return arry1;
        }
        public static string[] Extract(this string[] arry, string indexes, char separator)
        {
            if (arry.Length == 0 | arry == null) return null;
            var idList = indexes.ConvertIdsStringToIntegerList<int>(separator);
            var arry1 = new string[idList.Count];
            var i = 0;
            foreach (var id in idList)
            {
                arry1[i] = arry[id];
                i++;
            }
            return arry1;
        }


        public static string[] Wash(this string[] arry)
        {
            if (arry.Length == 0 | arry == null) return arry;
            arry = Trim(arry);
            arry = Clear(arry);
            return arry;
        }
        public static string[] Trim(this string[] arry)
        {
            if (arry.Length == 0 | arry == null) return arry;
            var arry1 = new string[arry.Length];
            for (int i = 0; i < arry.Length; i++)
            {
                arry1[i] = arry[i].Trim();
            }
            return arry1;
        }
        public static string[] Clear(this string[] arry)
        {
            if (arry.Length == 0 | arry == null) return arry;
            var list = new List<string>();
            for (int i = 0; i < arry.Length; i++)
            {
                if (!arry[i].IsNullOrEmptyOrWhiteSpace())
                {
                    list.Add(arry[i]);
                }
            }
            return list.ToArray();
        }

        public static string[] ToLower(this string[] arry)
        {
            if (arry.Length == 0 | arry == null) return arry;
            var list = new List<string>();
            for (int i = 0; i < arry.Length; i++)
            {
                list.Add(arry[i].ToLower());
            }
            return list.ToArray();
        }

        public static string Unwrap(this String[] strArry, string separator = "", int startIndex = 0, int len = 0)
        {
            if (strArry == null) return string.Empty;
            if (strArry.Length == 0) return string.Empty;
            if (len > strArry.Length) return string.Empty;
            if (len == 0) len = strArry.Length;

            string str = "";
            if (startIndex > strArry.Length - 1) return string.Empty;
            if (startIndex > len) return string.Empty;

            for (int i = startIndex; i < len; i++)
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




    }
}
