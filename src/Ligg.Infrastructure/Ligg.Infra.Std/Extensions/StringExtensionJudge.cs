using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Ligg.Infrastructure.Extensions
{
    public static partial class StringExtension
    {

        public static bool MatchesRegex(this string target, string rule)
        {
            if (string.IsNullOrEmpty(target))
            {
                var regex = new Regex(rule, RegexOptions.IgnoreCase);
                return Regex.IsMatch(target, rule);
            }
            return false;
        }

        public static bool IsLegalAbsolutePath(this string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }
            if (!target.Contains("\\")) return false;

            var fileName = "";
            try
            {
                fileName = Path.GetFileName(target);
            }
            catch (Exception ex)
            {
                return false;
            }
            if (!fileName.IsLegalFileName()) return false;

            var pathFirst2Letters = target.Substring(0, 2);
            if (pathFirst2Letters == "\\\\")
            {
                if (target.Contains(":"))
                {
                    return false;
                }
            }
            else
            {
                if (!target.Contains(":"))
                {
                    return false;
                }
                if (target.IndexOf(':') < 1)
                {
                    return false;
                }
                if (target.IndexOf(':') > 2)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsLegalFileName(this string target)
        {
            if (target.IndexOfAny(IllegalFileNameCharacters) > -1) return false;
            //if (FilenameExpression.IsMatch(target)) return true;
            return true;
        }

        public static bool IsLegalDirectory(this string target)
        {
            if (target.IndexOfAny(IllegalDirectoryCharacters) > -1) return false;
            //if (FilenameExpression.IsMatch(target)) return true;
            return true;
        }

        public static bool IsBeContainedInStringArray(this string target, string[] strArray, bool ignoreCase)
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



        public static bool IsWebUrl(this string target)
        {
            return !string.IsNullOrEmpty(target) && WebUrlExpression.IsMatch(target);
        }

        public static bool IsLegalEmailAddress(this string target)
        {
            return !string.IsNullOrEmpty(target) && EmailExpression.IsMatch(target);
        }

        public static bool IsPassword(this string target)
        {
            if (target.Length < 7) return false;
            return !string.IsNullOrEmpty(target) && PasswordExpression.IsMatch(target);
        }

        public static bool IsInteger(this string target)
        {
            return !string.IsNullOrEmpty(target) && (IntegerExpression.IsMatch(target));
        }

        public static bool IsPlusIntegerOrZero(this string target)
        {
            return !string.IsNullOrEmpty(target) && (PlusIntegerExpression.IsMatch(target) | target == "0");
        }

        public static bool IsPlusInteger(this string target)
        {
            return !string.IsNullOrEmpty(target) && PlusIntegerExpression.IsMatch(target);
        }

        public static bool IsNumeral(this string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }
            if (target.GetQtyOfIncludedChar('.') > 1) return false;
            if (target.StartsWith(".")) return false;
            if (target.GetQtyOfIncludedChar('-') > 1) return false;

            for (int i = 0; i < target.Length; i++)
            {
                if (i == 0)
                {
                    if (target[0] != '-' & !Char.IsNumber(target, i))
                        return false;
                }
                else
                {
                    if (target[i] != '.' & !Char.IsNumber(target, i))
                        return false;
                }
            }

            return true;
        }

        public static bool IsIpOrMask(this string target)
        {
            var strArry = new string[4];
            strArry = target.Split('.');
            if (strArry.Length != 4) return false;
            for (int i = 0; i < 4; i++)
            {
                if (!IsPlusIntegerOrZero(strArry[i])) return false;
                if (Convert.ToInt32(strArry[i]) > 255 | Convert.ToInt32(strArry[i]) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsIp(this string target)
        {
            var strArry = new string[4];
            strArry = target.Split('.');
            if (strArry.Length != 4) return false;
            for (int i = 0; i < 4; i++)
            {
                if (!IsPlusInteger(strArry[i])) return false;
                if (Convert.ToInt32(strArry[i]) >= 255 | Convert.ToInt32(strArry[i]) < 1)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsMask(this string target)
        {
            var strArry = new string[4];
            strArry = target.Split('.');
            if (strArry.Length != 4) return false;
            for (int i = 0; i < 4; i++)
            {
                if (!IsPlusIntegerOrZero(strArry[i])) return false;
                if (Convert.ToInt32(strArry[i]) > 255 | Convert.ToInt32(strArry[i]) < 0)
                {
                    return false;
                }
            }
            return true;
        }




    }
}