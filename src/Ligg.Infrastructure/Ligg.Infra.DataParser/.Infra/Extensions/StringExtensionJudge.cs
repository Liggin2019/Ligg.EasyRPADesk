using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Ligg.Infrastructure.Extensions
{
    internal static partial class StringExtension
    {

        internal static bool IsLegalFileName(this string target)
        {
            if (target.IndexOfAny(IllegalFileNameCharacters) > -1) return false;
            //if (FilenameExpression.IsMatch(target)) return true;
            return true;
        }

        internal static bool IsLegalDirectory(this string target)
        {
            if (target.IndexOfAny(IllegalDirectoryCharacters) > -1) return false;
            //if (FilenameExpression.IsMatch(target)) return true;
            return true;
        }

        internal static bool IsBeContainedInStringArray(this string target, string[] strArray, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            if (strArray == null)
            {
                return false;
            }
            if (ignoreCase)
                return strArray.Any(x => x.ToLower() == target.ToLower());
            else return strArray.Any(x => x == target);
        }


    }
}