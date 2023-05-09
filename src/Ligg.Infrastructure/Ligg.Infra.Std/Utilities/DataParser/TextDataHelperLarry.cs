
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

        //*larray
        public static string WashLarry(this string target)
        {
            if (target.IsNullOrEmpty()) return string.Empty;
            var arry = target.GetLarrayArray(true, false);
            var str1 = TextDataHelper.UnwrapLarray(arry);
            return str1;
        }

        public static string UnwrapLarray(this String[] strArry)
        {
            var exInfo = "\n>> " + _typeFullName + ".UnwrapLarray Error: ";
            var len = _parallelSeparators.Length;
            var separator = "";
            for (int i = len - 1; i > -1; i--)
            {
                if (strArry.Any(x => x.Contains(_parallelSeparators[i])))
                {
                    continue;
                }
                else
                {
                    separator = _parallelSeparators[i].ToString();
                    break;
                }
            }

            if (separator == "")
            {
                throw new ArgumentException(exInfo + "strArry should not contain all Parallel Separators");
            }
            return strArry.Unwrap(separator);
        }

        public static string ExtractLarray(this string target, string ids)
        {
            if (string.IsNullOrEmpty(target))
                return string.Empty;
            var strArry = target.GetLarrayArray(true, false);
            var separator = ids.GetParallelSeparator();
            strArry = strArry.Extract(ids, separator);
            return strArry.UnwrapLarray();
        }

        public static string[] GetLarrayArray(this string target, bool trim, bool clear)
        {
            if (target.IsNullOrEmpty()) return null;
            var separator = target.GetParallelSeparator();
            var arry = target.Split(separator);

            if (trim) arry = arry.Trim();
            if (clear) arry = arry.Clear();
            return arry;
        }

        public static string AddToLarry(this string target)
        {
            return "";
        }
        public static string RemoveFromLarry(this string target)
        {
            return "";
        }

        public static bool IsInLarry(this string target, string larry, bool ignoreCase)

        {
            if (target.IsNullOrEmpty()) return false;
            if (larry.IsNullOrEmpty()) return false;
            var arr = larry.GetLarrayArray(true, true);
            if (ignoreCase) return arr.Contains(target);
            else return arr.ToLower().Contains(target.ToLower());
        }

        public static UniversalResult ConvertLarryToUnivaralResult(string target, bool trim, bool clear)
        {
            if (target.IsNullOrEmpty()) return null;

            var rst = new UniversalResult();
            var arry = target.GetLarrayArray(trim, clear);
            if (arry != null)
            {
                rst.Success = arry[0].JudgeJudgementFlag();
                if (arry.Length > 1) rst.Message = arry[1];
            }
            return rst;
        }
        public static ScoreResult ConvertLarryToScoreResult(string target, bool trim, bool clear)
        {
            if (target.IsNullOrEmpty()) return null;

            var rst = new ScoreResult();
            rst.Score = -1;
            var arry = target.GetLarrayArray(trim, clear);
            if (arry != null)
            {
                if (arry[0].IsNumeral())
                    rst.Score = Convert.ToSingle(arry[0]);
                if (arry.Length > 1) rst.Message = arry[1];
            }
            return rst;
        }
















    }

}
