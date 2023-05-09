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
    internal static partial class StringExtension
    {

        internal static String[] SplitByString(this string target, string separator, bool clear, bool trim)
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


    
    }
}