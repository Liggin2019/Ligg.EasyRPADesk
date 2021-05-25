using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Ligg.Infrastructure.Base.Helpers;

namespace Ligg.Infrastructure.Base.Extension
{
    public static class StringExtension
    {
        //#regex
        private static readonly Regex WebUrlExpression = new Regex(@"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex EmailExpression = new Regex(@"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$", RegexOptions.Singleline | RegexOptions.Compiled);
        //必须字母、数字，特殊字符三者具备
        private static readonly Regex PasswordExpression = new Regex(@"^(?:(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])|(?=.*[A-Z])(?=.*[a-z])(?=.*[^A-Za-z0-9])|(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9])|(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9])).+", RegexOptions.Singleline | RegexOptions.Compiled);
        //必须字母、数字，特殊字符二者具备 has issue
        //private static readonly Regex PasswordExpression = new Regex(@"(?!^\\d+$)(?!^[a-zA-Z]+$)(?!^[_#@]+$).{7,}", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex StripHtmlExpression = new Regex("<\\S[^><]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex PlusIntegerExpression = new Regex("^[0-9]*[1-9][0-9]*$");
        private static readonly Regex AlphaAndNumeralExpressiown = new Regex("^[A-Za-z0-9]+$");
        private static readonly Regex AlphaNumeralUnderscoreHyphenAndChineseExpression = new Regex("^[A-Za-z0-9_\\-\u4e00-\u9fa5]+$");
        private static readonly Regex AlphaNumeralAndHyphenExpression = new Regex("^[A-Za-z0-9\\-]+$");
        private static readonly Regex ChineseExpression = new Regex("^[\u4e00-\u9fa5]+$ ");
        private static readonly char[] IllegalUrlCharacters = new[] { ';', '/', '\\', '?', ':', '@', '&', '=', '+', '$', ',', '<', '>', '#', '%', '.', '!', '*', '\'', '"', '(', ')', '[', ']', '{', '}', '|', '^', '`', '~', '–', '‘', '’', '“', '”', '»', '«' };
        private static readonly char[] IllegalFileNameCharacters = new[] { '<', '>', '/', '\\', '?', ':', '|', '*', '"' };


        //*common
        public static string FormatWith(this string target, params object[] args)
        {
            if (IsNullOrEmpty(target)) return string.Empty;
            return string.Format(CultureInfo.CurrentCulture, target, args);
        }

        public static string FormatWith1(this string target, params object[] args)
        {
            Check.Argument.IsNotEmpty(target, "target");

            return string.Format(CultureInfo.CurrentCulture, target, args);
        }

        public static string Hash(this string target)
        {
            if (IsNullOrEmpty(target)) return string.Empty;

            using (MD5 md5 = MD5.Create())
            {
                byte[] data = Encoding.Unicode.GetBytes(target);
                byte[] hash = md5.ComputeHash(data);
                return Convert.ToBase64String(hash);
            }
        }

        public static string WrapAt(this string target, int index)
        {
            const int DotCount = 3;

            if (IsNullOrEmpty(target)) return string.Empty;
            if (index <= 0) throw new ArgumentOutOfRangeException("index");

            return (target.Length <= index) ? target : string.Concat(target.Substring(0, index - DotCount), new string('.', DotCount));
        }

        public static string StripHtml(this string target)
        {
            return StripHtmlExpression.Replace(target, string.Empty);
        }

        public static string AddCharTillLength(this string target, int len, char addedChar)
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

        //*clean

        public static string CleanXmlInnerText(this string target)
        {
            if (string.IsNullOrEmpty(target)) return "";
            var outputStr = new StringBuilder();
            char ch;
            for (int i = 0; i < target.Length; i++)
            {
                ch = target[i];
                //d800 55296-dbff56319
                if ((ch >= 0x0020 && ch <= 0xD7FF) //55292
                    || (ch >= 0xE000 && ch <= 0xFFFD) //57344-65533
                    || ch == 0x0009 //t
                    || ch == 0x000A //10
                    || ch == 0x000D) //r
                {
                    outputStr.Append(ch);
                }
            }

            return outputStr.ToString();
        }

        //*split
        public static String[] SplitByTwoDifferentStrings(this string target, string seperator1, string seperator2, bool ignoreLastSeperator)
        {
            if (IsNullOrEmpty(target)) return null;
            if (ignoreLastSeperator)
            {
                if (!target.Contains(seperator1)) return null;

            }
            else
            {
                if (!target.Contains(seperator1) || !target.Contains(seperator2)) return null;
            }

            var strArry = target.SplitByString(seperator2);
            var strList = new List<string>();
            for (int i = 0; i < strArry.Length; i++)
            {
                if (!ignoreLastSeperator)
                {
                    if (i == strArry.Length - 1) break;
                }

                var tmpStr = strArry[i];
                if (tmpStr.Contains(seperator1))
                {
                    var qty = GetQtyOfIncludedString(tmpStr, seperator1);
                    var n = tmpStr.IndexOf(seperator1, qty - 1);
                    var subStr = tmpStr.Substring(n + seperator1.Length, tmpStr.Length - seperator1.Length - n);
                    if (!string.IsNullOrEmpty(subStr))
                    {
                        //strList.Add(subStr);
                    }
                    strList.Add(subStr);
                }
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

        public static String[] SplitByString(this string target, string seperator)
        {
            if (IsNullOrEmpty(target)) return null;
            //if (!target.Contains(seperator)) return null;
            var qty = GetQtyOfIncludedString(target, seperator);
            var result = new String[qty + 1];
            var str = target;
            for (int i = 1; i <= qty + 1; i++)
            {
                if (i != (qty + 1))
                {
                    var n = str.IndexOf(seperator, 0);
                    var subStr = str.Substring(0, n);
                    result[i - 1] = !string.IsNullOrEmpty(subStr) ? subStr : "";
                    str = str.Substring(n + seperator.Length, str.Length - n - seperator.Length);
                }
                else
                {
                    result[i - 1] = !string.IsNullOrEmpty(str) ? str : "";

                }
            }
            return result;
        }

        public static String[] SplitThenClear(this string target, char separator)
        {
            var arry = target.Split(separator);
            return StringHelper.ClearArray(arry);
        }

        public static String[] SplitThenTrim(this string target, char separator)
        {
            var arry = target.Split(separator);
            return StringHelper.TrimArray(arry);
        }
        public static String[] SplitThenWash(this string target, char separator)
        {
            var arry = target.Split(separator);
            return StringHelper.WashArray(arry);
        }

        public static String[] SplitByLastSeparator(this string target, char separator)
        {
            if (IsNullOrEmpty(target)) return null;
            var arry = target.Split(separator);
            var result = new string[2];
            for (int i = 0; i < arry.Length - 1; i++)
            {
                if (i == 0)
                {
                    result[0] = arry[i];
                }
                else
                {
                    result[0] = result[0] + separator.ToString() + arry[i];
                }
            }
            result[1] = arry[arry.Length - 1];
            return result;
        }

        //*join
        public static string AddOrDelToSeparatedStrings(this string target, string str, bool add, char separator)
        {
            if (add)
            {
                if (target.IsNullOrEmpty()) return str;
                if (str.IsNullOrEmpty()) return target;
                else
                {
                    var strArray = target.Split(separator);
                    if (strArray.Any(x => x.Equals(str)))
                    {
                        return target;
                    }
                    else
                    {
                        return (target + separator + str);
                    }
                }

            }
            else
            {
                if (target.IsNullOrEmpty()) return string.Empty;
                if (str.IsNullOrEmpty()) return target;
                else
                {
                    var strArray = target.Split(separator);
                    if (strArray.Any(x => x.Equals(str)))
                    {
                        var retStr = "";
                        var n = 0;
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            if (strArray[i] == str) continue;
                            if (n == 0)
                            {
                                retStr = strArray[i];
                            }
                            else
                            {
                                retStr = retStr + separator + strArray[i];
                            }
                            n++;
                        }
                        return retStr;
                    }
                    else
                    {
                        return target;
                    }
                }
            }

        }


        //*to
        public static String ToUniqueStringByShortGuid(this string target, string separator)
        {
            if (separator.IsNullOrEmpty()) separator = "-";
            var str = GetShortGuid();
            if (IsNullOrEmpty(target))
            {
                return str;
            }
            if (target.IsNullOrEmpty()) return str;
            return target + separator + str;
        }

        public static String ToUniqueStringByNow(this string target, string separator)
        {
            if (separator.IsNullOrEmpty()) separator = "-";
            if (IsNullOrEmpty(target)) return string.Empty;
            var str = DateTime.Now.ToString("yyMMddHHmmssfff");
            if (target.IsNullOrEmpty()) return str;
            return target + separator + str;
        }

        public static T ToEnum<T>(this string target, T defaultValue) where T : IComparable, IFormattable
        {
            T convertedValue = defaultValue;

            if (!string.IsNullOrEmpty(target))
            {
                try
                {
                    convertedValue = (T)Enum.Parse(typeof(T), target.Trim(), true);
                }
                catch (ArgumentException)
                {
                }
            }

            return convertedValue;
        }

        public static string ToLegalUrl(this string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return target;
            }

            target = target.Trim();
            target = target.Replace(".", "-");
            if (target.IndexOfAny(IllegalUrlCharacters) > -1)
            {
                foreach (char character in IllegalUrlCharacters)
                {
                    target = target.Replace(character.ToString(CultureInfo.CurrentCulture), string.Empty);
                }
            }

            target = target.Replace(" ", "-");

            while (target.Contains("--"))
            {
                target = target.Replace("--", "-");
            }

            return target;
        }

        public static string ToLegalFileName(this string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return target;
            }

            target = target.Trim();
            target = target.Replace(" ", "-");
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

        //#Convert
        public static string ConvertDecimalToBinaryCode(this string target)
        {
            try
            {
                if (!target.IsPlusInteger()) return string.Empty;

                var strL = long.Parse(target);
                var rst = Convert.ToString(strL, 2);
                return rst;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string ConvertBinaryToDecimalCode(this string target)
        {
            try
            {
                if (!target.IsBinaryCode()) return string.Empty;
                var rst = "";
                double double1 = 0;
                double doubleTmp = 0;
                var n = target.Length;
                for (int i = 0; i < target.Length; i++)
                {
                    var n1 = n - i - 1;
                    if (target[i] == '0')
                        doubleTmp = 0;
                    else doubleTmp = Math.Pow(2, n1);
                    double1 = double1 + doubleTmp;
                }

                rst = Convert.ToString(double1);
                return rst;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static Guid ConvertBase64StringToGuid(this string target)
        {
            Guid result = Guid.Empty;

            if ((!string.IsNullOrEmpty(target)) && (target.Trim().Length == 22))
            {
                string encoded = string.Concat(target.Trim().Replace("-", "+").Replace("_", "/"), "==");

                try
                {
                    byte[] base64 = Convert.FromBase64String(encoded);

                    result = new Guid(base64);
                }
                catch (FormatException)
                {
                }
            }

            return result;
        }

        public static List<T> ConvertIdsStringToIntegerList<T>(this string target, char separator)
        {
            var IntegerList = new List<T>();
            var idStrArray = target.Split(separator);
            if (!(idStrArray.Length == 1 && idStrArray[0] == ""))
            {
                if (idStrArray.Length > 0)
                {
                    for (var i = 0; i < idStrArray.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(idStrArray[i]))
                        {
                            var integer = (T)Convert.ChangeType(idStrArray[i], typeof(T));
                            IntegerList.Add(integer);
                        }
                    }
                }
            }
            return IntegerList;
        }




        public static Object ConvertToAnyType(this String target, Type type, char spacingChar, char lineBreakChar)
        {
            if (String.IsNullOrEmpty(target))
                return null;
            if (type == null)
                return target;
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                String[] strs = target.Split(new char[] { lineBreakChar });
                Array array = Array.CreateInstance(elementType, strs.Length);
                for (int i = 0, c = strs.Length; i < c; ++i)
                {
                    var tempStr = strs[i].Replace(spacingChar.ToString(), " ");
                    array.SetValue(ObjectHelper.ConvertToAnyType(tempStr, elementType), i);
                }
                return array;
            }
            return ObjectHelper.ConvertToAnyType(target, type);
        }



        public static string ConvertFirstLetterToUpper(this string target)
        {
            string fl = target.Substring(0, 1);
            fl = fl.ToUpper();
            target = fl + target.Substring(1, target.Length - 1);
            return target;
        }

        public static string ConvertNullToEmpty(this string target)
        {
            return (target ?? string.Empty).Trim();
        }


        //*get
        public static string GetShortGuid()
        {
            var guid = Guid.NewGuid();
            return guid.ToBase64String();
        }

        public static int GetQtyOfIncludedChar(this string target, char incChar)
        {
            int count = 0;
            for (int i = 0; i < target.Length; i++)
            {
                if (target[i] == incChar)
                {
                    count++;
                }
            }
            return count;
        }

        public static int GetQtyOfIncludedString(this string target, string incStr)
        {
            var schStr = incStr.Trim();
            if (string.IsNullOrEmpty(schStr)) return 0;
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

        public static string GetFirstSeparatedString(this string target, char separator)
        {
            if (IsNullOrEmpty(target)) return null;
            var arry = target.Split(separator);
            if (arry.Length == 1) return null;
            return arry[0];
        }

        public static string GetLastSeparatedString(this string target, char separator)
        {
            if (IsNullOrEmpty(target)) return null;
            var arry = target.Split(separator);
            if (arry.Length == 1) return null;
            return arry[arry.Length - 1];
        }

        public static string GetStyleValue(this string target, string styleName)
        {
            if (string.IsNullOrEmpty(target))
                return string.Empty;
            //var val = target.ToLower().SplitByTwoDifferentStrings(styleName.ToLower() + ":", ";", true);
            var val = target.SplitByTwoDifferentStrings(styleName + ":", ";", true);
            if (val != null) return val[0].Trim();
            return string.Empty;
        }

        //*judge
        public static bool IsNullOrEmpty(this string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return true;
            }
            return false;
        }

        public static bool MatchesRegex(this string target, string rule)
        {
            if (string.IsNullOrEmpty(target))
            {
                var regex = new Regex(rule, RegexOptions.IgnoreCase);
                return Regex.IsMatch(target, rule);
            }
            return false;
        }

        public static bool MatchesWildCard(this string target, string rule)
        {
            if (string.IsNullOrEmpty(target))
            {
                var regex = new Regex(rule, RegexOptions.IgnoreCase);
                return Regex.IsMatch(target, rule);
            }
            return false;
        }


        public static bool IsBeContainedInStringAfterSplitWithSeparator(this string target, string str, char separator)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            var strArray = str.Split(separator);
            return strArray.Any(x => x == target);
        }
        public static bool IsBeContainedInStringArray(this string target, string[] strArray)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            if (strArray == null)
            {
                return false;
            }

            return strArray.Any(x => x == target);
        }

        public static bool IsIntersectingStringAfterSplitWithSeparator(this string target, string str, char separator)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            var idStrArray1 = target.Split(separator);
            var idStrArray2 = str.Split(separator);
            return idStrArray2.Any(t2 => idStrArray1.Any(t1 => t2 == t1));
        }

        public static bool IsDirectoryOrFilePath(this string target)
        {
            return FileHelper.IsAbsolutePath(target);
        }

        public static bool IsWebUrl(this string target)
        {
            return !string.IsNullOrEmpty(target) && WebUrlExpression.IsMatch(target);
        }


        public static bool IsEmailAddress(this string target)
        {
            return !string.IsNullOrEmpty(target) && EmailExpression.IsMatch(target);
        }

        public static bool IsPassword(this string target)
        {
            if (target.Length < 7) return false;
            return !string.IsNullOrEmpty(target) && PasswordExpression.IsMatch(target);
        }

        public static bool IsPlusIntegerOrZero(this string target)
        {
            return !string.IsNullOrEmpty(target) && (PlusIntegerExpression.IsMatch(target) | target == "0");
        }

        public static bool IsPlusInteger(this string target)
        {
            return !string.IsNullOrEmpty(target) && PlusIntegerExpression.IsMatch(target);
        }

        //*same as IsPlusInteger
        private static bool IsNumber(this string target)
        {
            for (int i = 0; i < target.Length; i++)
            {
                if (!Char.IsNumber(target, i))
                    return false;
            }
            return true;
        }
        public static bool JudgeNumeralRange(this string target, string min, string max)
        {
            if (target.IsNumeral() & max.IsNumeral() & min.IsNumeral())
            {
                var targetInt = Convert.ToDouble(target);
                if (targetInt > Convert.ToDouble(min) & targetInt < Convert.ToDouble(max))
                    return true;
            }
            else if (target.IsNumeral() & max.IsNumeral() & min.IsNullOrEmpty())
            {
                var targetInt = Convert.ToDouble(target);
                if (targetInt < Convert.ToDouble(max))
                    return true;
            }
            else if (target.IsNumeral() & max.IsNullOrEmpty() & min.IsNumeral())
            {
                var targetInt = Convert.ToDouble(target);
                if (targetInt > Convert.ToDouble(min))
                    return true;
            }

            return false;
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

        public static bool IsBinaryCode(this string target)
        {
            for (int i = 0; i < target.Length; i++)
            {
                if (target[i] != '0' & target[i] != '1') return false;
            }
            return true;
        }


        public static bool IsLegalUrl(this string target)
        {
            if (target.Contains(" "))
            {
                return false;
            }
            if (target.IndexOfAny(IllegalUrlCharacters) > -1)
            {
                return false;
            }
            return true;
        }

        public static bool IsLegalFileName(this string target)
        {
            if (target.IndexOfAny(IllegalFileNameCharacters) > -1) return false;
            //if (FilenameExpression.IsMatch(target)) return true;
            return true;
        }


        public static bool IsAlphaAndNumeral(this string target)
        {
            return !string.IsNullOrEmpty(target) && AlphaAndNumeralExpressiown.IsMatch(target);
        }

        public static bool IsAlphaNumeralAndHyphenOrEmpty(this string target)
        {
            if (target.IsNullOrEmpty()) return true;
            return AlphaNumeralAndHyphenExpression.IsMatch(target);
        }

        public static bool IsAlphaNumeralAndHyphen(this string target)
        {
            if (target.IsNullOrEmpty()) return false;
            return AlphaNumeralAndHyphenExpression.IsMatch(target);
        }


        public static bool IsOutOfLength(this string target, int length)
        {
            if (target.Trim().Length > length)
            {
                return true;
            }
            return false;
        }

        public static bool IsJson(this string target)
        {
            if (target.IsNullOrEmpty()) return false;
            if (target.Contains("{") & target.Contains("}") & target.Contains(":")) return true;
            return false;
        }
        public static bool IsEntityJson(this string target)
        {
            return !target.IsListJson();
        }
        public static bool IsListJson(this string target)
        {
            if (!target.IsJson()) return false;
            if (target.Contains("[") & target.Contains("]")) return true;
            return false;
        }





    }
}