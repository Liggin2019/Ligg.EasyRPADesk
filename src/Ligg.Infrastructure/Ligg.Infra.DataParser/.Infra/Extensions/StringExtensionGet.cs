using System;
using System.Linq;

namespace Ligg.Infrastructure.Extensions
{
    internal static partial class StringExtension
    {

        internal static int GetQtyOfIncludedString(this string target, string incStr)
        {
            var schStr = incStr;
            if (incStr == null) return 0;
            if (string.IsNullOrEmpty(target)) return 0;
            int schStrLen = schStr.Length;
            var targetLen = target.Length;
            int index = 0;
            int pos = 0;
            int count = 0;
            do
            {
                index = target.IndexOf(schStr, pos);
                if (index != -1)
                {
                    count++;
                }
                pos = schStrLen + index;
            } while (index != -1 && pos + schStrLen < targetLen + 1);
            return count;
        }


        internal static string GetSubStringSurroundedByTwoDifferentIdentifiers(this string target, string identifier, string identifier1, int index=0,bool ignoreCase=false)
        {
            if (string.IsNullOrEmpty(target)) return string.Empty;
            var arr = target.ExtractSubStringsByTwoDifferentItentifiers(identifier, identifier1, ignoreCase);
            if (arr != null)
            {
                
                if (arr.Length > index)
                {
                    return arr[index];
                }
            }
               
            return string.Empty;
        }




    }
}