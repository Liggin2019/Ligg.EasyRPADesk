using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Ligg.Infrastructure.Extensions
{
    internal static partial class StringExtension
    {

        //*add
        internal static string AddCharTillLength(this string target, int len, char addedChar)
        {
            if (target.Length < len)
            {
                target = addedChar + target;
            }
            if (target.Length == len)
            {
                return target;
            }
            target = AddCharTillLength(target, len, addedChar);
            return target;
        }


        internal static String[] ExtractSubStringsByTwoDifferentItentifiers(this string target, string seperator1, string seperator2, bool ignoreCase)
        {
            if (IsNullOrEmpty(target)) return null;
            if (seperator1.IsNullOrEmpty()) return null;
            if (seperator2.IsNullOrEmpty()) return null;

            if (ignoreCase)
            {
                seperator1 = seperator1.ToLower();
                seperator2 = seperator2.ToLower();
                if (!target.ToLower().Contains(seperator1)) return null;
            }
            else
            {
                if (!target.Contains(seperator1)) return null;
            }


            var strArry = target.SplitByString(seperator2, true, false);
            var strList = new List<string>();
            for (int i = 0; i < strArry.Length; i++)
            {
                var tmpStr = strArry[i];
                var tmpStr1 = ignoreCase ? tmpStr.ToLower() : tmpStr;
                if (tmpStr1.Contains(seperator1))
                {
                    //var tmpStr2Arry = tmpStr1.SplitByString(seperator1, true, false);
                    //if (tmpStr2Arry.Length == 1)
                    //    strList.Add(tmpStr2Arry[0]);
                    //else
                    //{

                    //}

                    var qty = GetQtyOfIncludedString(tmpStr1, seperator1);
                    var n = tmpStr1.IndexOf(seperator1, qty - 1);
                    var subStr = tmpStr.Substring(n + seperator1.Length, tmpStr.Length - seperator1.Length - n);
                    strList.Add(subStr);

                }
                /*
                if (tmpStr1.StartsWith(seperator1))
                {
                    var n = 0;
                    var subStr = tmpStr.Substring(n + seperator1.Length, tmpStr.Length - seperator1.Length - n);
                    strList.Add(subStr);
                }
                */
            }

            if (strList.Count > 0)
            {
                var result = new String[strList.Count()];
                for (int i = 0; i < strList.Count; i++)
                {
                    result[i] = strList[i];
                }
                return result;
            }
            return null;
        }



    }
}