using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;


namespace Ligg.WinFormBase
{
    public static class StyleHelper
    {
        private static readonly char[] _parallelSeparators = new char[] { '`', '!', '|', ',', '*', ' ' };  //取消 '\\', '/' ;, '&'; remark: xml读取&报错; 
        private static readonly char[] _keyValueSeparators = new char[] { '~', '=', '_', '-', ':' };//取消 '<', '>'  remark: xml不支持:>输不进去； < &读取报错 XmlHelper.ConvertToGeneric Error: XML 文档(41, 126)中有错误。
        public static string GetStyleValue(this string target, string key)
        {
            if (string.IsNullOrEmpty(target))
                return string.Empty;
            var arr= target.GetLdictArray(true, true);
            var val= GetLdictValue(arr, key, false);
            return val;
        }

        public static string GetRgbLarray(this Color target, char separator = ',')
        {
            if (target == null) return string.Empty;
            if (!_parallelSeparators.Contains(separator)) separator = ',';
            var rst = target.R.ToString() + separator + target.G.ToString() + separator + target.B;
            return rst;
        }
        public static Color GetColorFormLarray(this string target)
        {
            Color color = Color.Transparent;
            var colorStr = target;
            if (!string.IsNullOrEmpty(colorStr))
            {
                var colorStrArray = colorStr.GetLarrayArray(true, true);
                if (colorStrArray.Length == 3 )
                {
                    if(colorStrArray[0].IsPlusIntegerOrZero() & colorStrArray[1].IsPlusIntegerOrZero() & colorStrArray[2].IsPlusIntegerOrZero())
                    {
                        var r = Convert.ToInt32(colorStrArray[0]);
                        var g = Convert.ToInt32(colorStrArray[1]);
                        var b = Convert.ToInt32(colorStrArray[2]);
                        color = Color.FromArgb(r > 255 ? 255 : r, g > 255 ? 255 : g, b > 255 ? 255 : b);
                    }
                }
            }
            return color;

        }
        private static string[] GetLdictArray(this string target, bool trim, bool clearReturnChar)
        {
            if (string.IsNullOrEmpty(target)) return null;
            if (clearReturnChar)
                target = target.Replace("\n", "");
            var strArry = target.GetLarrayArray(trim, true);
            return strArry;
        }
        private static char GetLdictSeparator(this string target)
        {
            return GetSeparator(target, _keyValueSeparators);
        }

        private static string GetLdictValue(this string[] ldictArray, string key, bool ignoreCase)
        {
            if (ldictArray == null) return string.Empty;
            if (ldictArray.Length == 0) return string.Empty;
            var strArry = ldictArray;
            for (int i = 0; i < strArry.Length; i++)
            {
                var strArry1 = GetLdictKeyValuePair(strArry[i]);

                if (ignoreCase)
                {
                    if (key.ToLower() == strArry1[0].ToLower())
                    {
                        if (strArry1.Length == 1) return null;
                        return strArry1[1];
                    }

                }
                else
                {
                    if (key == strArry1[0])
                    {
                        if (strArry1.Length == 1) return null;
                        return strArry1[1];
                    }
                }
            }
            return string.Empty;
        }

        private static string[] GetLdictKeyValuePair(string target)
        {
            var separator = target.GetLdictSeparator();
            var arry = target.Split(separator);
            return arry;
        }

        internal static string[] GetLarrayArray(this string target, bool trim, bool clear)
        {
            if (string.IsNullOrEmpty(target)) return null;
            var separator = target.GetLarraySeparator();
            var arry = target.Split(separator);

            if (trim) arry = arry.Trim();
            if (clear) arry = arry.Clear();
            return arry;
        }
        private static char GetLarraySeparator(this string target)
        {
            return GetSeparator(target, _parallelSeparators);
        }

        private static char GetSeparator(string target, char[] separators)
        {
            var len = separators.Length;
            var separator = separators[len - 1];
            if (string.IsNullOrEmpty(target)) return separator;
            for (int i = 0; i < len; i++)
            {
                if (target.Contains(separators[i]))
                {
                    return separators[i];
                }
            }

            return separator;
        }


    }

    //*class

    internal static class TextDataHelperExtension
    {
        internal static string[] Trim(this string[] arry)
        {
            if (arry.Length == 0 | arry == null) return arry;
            var arry1 = new string[arry.Length];
            for (int i = 0; i < arry.Length; i++)
            {
                arry1[i] = arry[i].Trim();
            }
            return arry1;
        }
        internal static string[] Clear(this string[] arry)
        {
            if (arry.Length == 0 | arry == null) return arry;
            var list = new List<string>();
            for (int i = 0; i < arry.Length; i++)
            {
                if (!string.IsNullOrEmpty(arry[i]))
                {
                    list.Add(arry[i]);
                }
            }
            return list.ToArray();
        }

        internal static int GetQtyOfIncludedChar(this string target, char incChar)
        {
            int count = 0;
            for (int i = 0; i < target.Length; i++)
            {
                if (target[i] == incChar)
                {
                    count++;
                }
            }
            return count;
        }

        public static bool IsPlusIntegerOrZero(this string target)
        {
            return !string.IsNullOrEmpty(target) && (PlusIntegerExpression.IsMatch(target) | target == "0");
        }
        private static readonly Regex PlusIntegerExpression = new Regex("^[0-9]*[1-9][0-9]*$");
    }
}
