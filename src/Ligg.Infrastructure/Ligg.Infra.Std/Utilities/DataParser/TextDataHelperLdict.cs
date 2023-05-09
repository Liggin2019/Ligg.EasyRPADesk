
using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace Ligg.Infrastructure.Utilities.DataParserUtil
{
    public static partial class TextDataHelper
    {
        public static Dictionary<string, string> ConvertLdictToDictionary(string target, bool trim, bool clear, bool clearReturnChar)
        {
            if (target.IsNullOrEmpty()) return null;
            var arry = target.GetLdictArray(false, clearReturnChar);
            var dict = new Dictionary<string, string>();
            foreach (var str in arry)
            {
                var arr1 = GetLdictKeyValuePair(str);
                if (arr1.Length < 2) continue;
                if (arr1[0].IsNullOrEmptyOrWhiteSpace()) continue;
                if (clear) if (arr1[1].IsNullOrEmpty()) continue;
                if (!dict.ContainsKey(arr1[0].Trim()))
                    dict.Add(arr1[0].Trim(), trim ? arr1[1].Trim() : arr1[1]);
            }
            return dict;
        }

        public static string GetLdictValue(this Dictionary<string, string> dict, string key)
        {
            if (dict == null) return string.Empty;
            if (key.IsNullOrEmpty()) return string.Empty;
            if (!dict.ContainsKey(key)) return string.Empty;
            return dict[key];

        }
        public static string AddToLdict(this string target, string key, string value, bool trim, bool clear)
        {
            if (target == null) return string.Empty;
            var dict = ConvertLdictToDictionary(target, trim, clear, true);
            dict.Add(key, value);
            return GetLdictFromDictionary(dict);
        }

        public static string GetLdictValue(this string target, string key, bool trim, bool clear)
        {
            if (string.IsNullOrEmpty(target))
                return string.Empty;
            var dict = ConvertLdictToDictionary(target, trim, clear, true);
            if (dict != null) return dict.GetLdictValue(key);
            return string.Empty;
        }
        public static string GetHttpClientParamString(string[] paramArr)
        {
            if (paramArr != null)
            {
                var dict = new Dictionary<string, string>();
                foreach (string str in paramArr)
                {
                    var array = str.GetLdictArray(true, true);
                    foreach(var keyValue in array)
                    {
                        var arr = GetLdictKeyValuePair(keyValue);
                        if (arr.Length > 1)
                            dict.Add(arr[0], arr[1] ?? "");
                    }
                }
                int i = 0;
                var rst = "";
                foreach (var key in dict.Keys)
                {
                    rst = i == 0 ? key + "=" + dict.GetLdictValue(key) : rst+"&" + key + "=" + dict.GetLdictValue(key);
                    i++;
                }
                return rst;
            }

            return string.Empty;
        }

        //*private
        private static string[] GetLdictArray(this string target, bool trim, bool clearReturnChar)
        {
            if (target.IsNullOrEmpty()) return null;
            if (clearReturnChar)
                target = target.Replace("\n", "");
            var strArry = target.GetLarrayArray(trim, true);
            return strArry;
        }

        private static string GetLdictFromDictionary(Dictionary<string, string> dict)
        {
            if (dict == null) return string.Empty;
            var str = "";
            foreach (var dictItem in dict)
            {
                str = str + dictItem.Key + dictItem.Value;

            }

            var minKeyValueSeparator = GetMinSeparator(str, _keyValueSeparators);
            if (minKeyValueSeparator.IsEmpty()) throw new ArgumentException("GetMinKeyValueSeparator failed: " + "string= " + str);
            var minParallelSeparator = GetMinSeparator(str, _parallelSeparators);
            if (minParallelSeparator.IsEmpty()) throw new ArgumentException("GetMinParallelSeparator failed: " + "string= " + str);

            var rst = "";
            var i = 0;
            foreach (var dictItem in dict)
            {
                rst = i == 0 ? dictItem.Key + minKeyValueSeparator + dictItem.Value :
                    rst + minParallelSeparator + dictItem.Key + minKeyValueSeparator + dictItem.Value;
                i++;
            }

            return rst;
        }

        //*old
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

        //public static string GetLdictValue(this string target, string key, bool ignoreCase, bool trim, bool clearReturnChar)
        //{
        //    if (string.IsNullOrEmpty(target))
        //        return string.Empty;
        //    var strArry = target.GetLdictArray(trim, clearReturnChar);
        //    for (int i = 0; i < strArry.Length; i++)
        //    {
        //        var strArry1 = GetLdictKeyValuePair(strArry[i]);
        //        if (ignoreCase)
        //        {
        //            if (key.ToLower() == strArry1[0].ToLower())
        //            {
        //                if (strArry1.Length == 1) return null;
        //                return strArry1[1];
        //            }

        //        }
        //        else
        //        {
        //            if (key == strArry1[0])
        //            {
        //                if (strArry1.Length == 1) return null;
        //                return strArry1[1];
        //            }
        //        }
        //    }
        //    return string.Empty;
        //}



    }
}
