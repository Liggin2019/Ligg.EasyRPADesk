using Ligg.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Ligg.Infrastructure.Extensions
{
    public static partial class StringExtension
    {
        //*to
        public static String ToUniqueStringByShortGuid(this string target, string separator)
        {
            if (separator.IsNullOrEmpty()) separator = "";
            if (target.IsNullOrEmpty()) target = "";
            var str = Guid.NewGuid().GetShortGuid();
            return target + separator + str;
        }

        public static String ToUniqueStringByNow(this string target, string separator)
        {
            if (separator.IsNullOrEmpty()) separator = "";
            if (target.IsNullOrEmpty()) target = "";
            var str = DateTime.Now.ToString("yyMMddHHmmssfff");
            return target + separator + str;
        }
        public static String ToUniqueStringByUtcNow(this string target, string separator)
        {
            if (separator.IsNullOrEmpty()) separator = "-";
            var str = DateTime.UtcNow.ToString("yyMMddHHmmssfff");
            if (target.IsNullOrEmpty()) return str;
            return target + separator + str;
        }

        public static String ToUniqueStringByRandomInteger(this string target, int min, int max, string separator)
        {
            if (separator.IsNullOrEmpty()) separator = "";
            if (target.IsNullOrEmpty()) target = "";
            var str = new Random().Next(min, max).ToString();
            return target + separator + str;
        }


        public static String ToUniqueStringByAutoIncrementInteger(this string target, int baseValue, string separator)
        {
            if (separator.IsNullOrEmpty()) separator = "";
            if (target.IsNullOrEmpty()) target = "";
            var str = (baseValue + 1).ToString();
            return target + separator + str;
        }



        public static string ToLegalFileName(this string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return target;
            }

            target = target.Trim();
            target = target.Replace(" ", "-");
            target = target.Replace(",", "-");
            if (target.IndexOfAny(IllegalFileNameCharacters) > -1)
            {
                foreach (char character in IllegalFileNameCharacters)
                {
                    target = target.Replace(character.ToString(CultureInfo.CurrentCulture), "-");
                }
            }
            return target;

        }

        public static Char ToChar(this string target)
        {
            Char result = ' ';
            if (target == "\\n") result = '\n';
            else if (target == "\\t") result = '\t';
            else if (target == "\\r") result = '\r';
            else result = Convert.ToChar(target);
            return result;
        }



    }
}