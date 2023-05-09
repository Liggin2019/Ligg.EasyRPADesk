using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using Ligg.Infrastructure.Helpers;

namespace Ligg.Infrastructure.Extensions
{
    public static partial class StringExtension
    {

        public static String[] SplitByString(this string target, string separator, bool clear, bool trim)
        {
            if (IsNullOrEmpty(target)) return null;
            //if (!target.Contains(seperator)) return null;
            var qty = GetQtyOfIncludedString(target, separator);
            var result = new String[qty + 1];
            var str = target;
            for (int i = 1; i <= qty + 1; i++)
            {
                if (i != (qty + 1))
                {
                    var n = str.IndexOf(separator, 0);
                    var subStr = str.Substring(0, n);
                    result[i - 1] = !string.IsNullOrEmpty(subStr) ? subStr : "";
                    str = str.Substring(n + separator.Length, str.Length - n - separator.Length);
                }
                else
                {
                    result[i - 1] = !string.IsNullOrEmpty(str) ? str : "";

                }
            }
            if (clear) result = StringArrayExtension.Clear(result);
            if (trim) result = StringArrayExtension.Trim(result);
            return result;
        }

        public static String[] SplitByChar(this string target, char separator, bool clear, bool trim)
        {
            var arry = target.Split(separator);
            if (clear) arry = StringArrayExtension.Clear(arry);
            if (trim) arry = StringArrayExtension.Trim(arry);
            return arry;
        }

    
    }
}