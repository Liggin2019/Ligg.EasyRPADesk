
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
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        //` ~ 最后再用
        //取消 - 这个太常用; _ 可以考虑給_keyValueSeparators
        //取消 '<', '>','&'  remark: xml不支持:>输不进去； < &读取报错 XmlHelper.ConvertToGeneric Error: XML 文档(41, 126)中有错误。
        private static readonly char[] _parallelSeparators = new char[] { '!', '|', ',', '*', ' ' };

        private static readonly char[] _keyValueSeparators = new char[] { '=', ':' };
        private static readonly char[] _dataSeparators = new char[] { '\n' };

        public static string JudgeUniversalResult(this string target)
        {
            if (target.IsNullOrEmpty()) return false.ToString();
            try
            {
                var universalResult = JsonHelper.ConvertToGeneric<UniversalResult>(target);
                return universalResult.Success.ToString();
            }
            catch { return false.ToString(); }
        }

        public static string ValidateTotal(string[] arr, TernaryOption index)//1:And, 0:Or, -1:Not
        {
            if (arr == null) return null;
            try
            {
                if (index == TernaryOption.Forward)
                {
                    foreach (var str in arr)
                    {
                        var universalResult = JsonHelper.ConvertToGeneric<UniversalResult>(str);
                        if (universalResult.Success == false) return str;
                    }
                }
                else if (index == TernaryOption.Backward)
                {
                    foreach (var str in arr)
                    {
                        var universalResult = JsonHelper.ConvertToGeneric<UniversalResult>(str);
                        if (universalResult.Success == true) return str;
                    }
                }
                else
                {
                    var universalResult = JsonHelper.ConvertToGeneric<UniversalResult>(arr[0]);
                    universalResult.Success = !universalResult.Success;
                    return GenericHelper.ConvertToJson<UniversalResult>(universalResult);
                }

                return null;

            }
            catch { return null; }
        }

        public static List<KeyValue> ConvertToKeyValueList(string target, TxtDataType txtDataFormat, bool trim, bool clear)
        {
            if (target.IsNullOrEmpty()) return null;

            var keyVals = new List<KeyValue>();
            if (txtDataFormat == TxtDataType.Larray)
            {
                var arry = target.GetLarrayArray(trim, clear);
                if (arry != null)
                {
                    for (int i = 0; i < arry.Length; i++)
                    {
                        var keyVal = new KeyValue();
                        keyVal.Key = i.ToString();
                        keyVal.Value = arry[i];
                        keyVals.Add(keyVal);
                    }
                }
            }
            else if (txtDataFormat == TxtDataType.Ldict)
            {
                var arry = target.GetLarrayArray(false, false);
                if (arry != null)
                {
                    foreach (var str in arry)
                    {
                        var arry1 = GetLdictKeyValuePair(str);
                        var keyVal = new KeyValue();
                        if (arry1[0].IsNullOrEmptyOrWhiteSpace()) continue;
                        keyVal.Key = arry1[0].Trim();
                        if (clear) if (arry1[1].IsNullOrEmpty()) continue;
                        keyVal.Value = trim ? arry1[1].Trim() : arry1[1];
                        keyVals.Add(keyVal);
                    }
                }
            }
            else throw new NotImplementedException(txtDataFormat.ToString());
            return keyVals;
        }


        //*private

        private static char GetSeparator(this string target, char[] separators)
        {
            var len = separators.Length;
            var separator = separators[len - 1];
            if (target.IsNullOrEmpty()) return separator;
            for (int i = 0; i < len; i++)
            {
                if (target.Contains(separators[i]))
                {
                    return separators[i];
                }
            }

            return separator;
        }
        private static string GetMinSeparator(string target, char[] separators)
        {
            var minSeparator = "";
            for (var i = separators.Length - 1; i > -1; i--)
            {
                var included = false;
                for (var j = 0; j < i + 1; j++)
                {
                    if (target.Contains(separators[j])) included = true;
                }
                if (!included)
                {
                    minSeparator = separators[i].ToString();
                    break;
                }
            }
            //if (minSeparator == "") throw new ArgumentException("GetMinSeparator failed: " + "string= " + target);
            return minSeparator;
        }



        private static char GetParallelSeparator(this string target)
        {
            return target.GetSeparator(_parallelSeparators);
        }

        private static bool ContainsParallelSeparator(this string target)
        {
            if (string.IsNullOrEmpty(target)) return false;
            foreach (var separator in _parallelSeparators)
            {
                if (target.Contains(separator)) return true;
            }
            return false;
        }

        private static char GetDataSeparator(this string target)
        {
            var separator = _dataSeparators[0];
            return separator;
        }
        private static string[] GetLdictKeyValuePair(string target)
        {
            var separator = target.GetKeyValueSeparator();
            var arry = target.Split(separator);
            return arry;
        }

        private static char GetKeyValueSeparator(this string target)
        {
            var separator = target.GetSeparator(_keyValueSeparators);
            return separator;
        }

        //*common
        public static string GetJudgementFlag(this string target)
        {
            if (target.IsNullOrEmpty()) return "false";
            if (target.ToLower() == "true") return "true";
            return "false";
        }

        public static bool JudgeJudgementFlag(this string target)
        {
            if (target.IsNullOrEmpty()) return false;
            if (target.ToLower() == "true") return true;
            return false;
        }







    }
}
