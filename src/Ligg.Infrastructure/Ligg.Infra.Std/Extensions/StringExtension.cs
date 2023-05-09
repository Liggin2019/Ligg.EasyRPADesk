using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Ligg.Infrastructure.Extensions
{
    public static partial class StringExtension
    {
        //*regex
        private static readonly Regex WebUrlExpression = new Regex(@"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex EmailExpression = new Regex(@"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$", RegexOptions.Singleline | RegexOptions.Compiled);
        //必须字母、数字，特殊字符三者具备
        private static readonly Regex PasswordExpression = new Regex(@"^(?:(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])|(?=.*[A-Z])(?=.*[a-z])(?=.*[^A-Za-z0-9])|(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9])|(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9])).+", RegexOptions.Singleline | RegexOptions.Compiled);
        //必须字母、数字，特殊字符二者具备 has issue
        //private static readonly Regex PasswordExpression = new Regex(@"(?!^\\d+$)(?!^[a-zA-Z]+$)(?!^[_#@]+$).{7,}", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex PlusIntegerExpression = new Regex("^[0-9]*[1-9][0-9]*$");
        private static readonly Regex IntegerExpression = new Regex("^-?[1-9]\\d*$");
        public static readonly Regex AlphaNumeralUnderscoreHyphenAndChineseExpression = new Regex("^[A-Za-z0-9_\\-\u4e00-\u9fa5]+$");
        public static readonly Regex AlphaAndNumeralExpression = new Regex("^[A-Za-z0-9]+$");
        public static readonly Regex AlphaNumeralAndHyphenExpression = new Regex("^[A-Za-z0-9\\-]+$");
        public static readonly Regex AlphaNumeralHyphenAndDotExpression = new Regex("^[A-Za-z0-9\\-\\.]+$");
        public static readonly Regex ChineseExpression = new Regex("^[\u4e00-\u9fa5]+$ ");
        public static readonly Regex EnglishExpression = new Regex(@"^[A-Za-z]+$");
        private static readonly char[] IllegalFileNameCharacters = new[] { '\r', '\t', '\n', '<', '>', '/', '\\', '?', ':', '|', '*', '"' };
        private static readonly char[] IllegalDirectoryCharacters = new[] { '\r', '\t', '\n', '<', '>', '/', '?', '|', '*', '"' };

        //*common
        internal static bool IsNullOrEmptyOrWhiteSpace(this string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return true;
            }
            if (string.IsNullOrWhiteSpace(target))
            {
                return true;
            }
            return false;

        }

        public static bool IsNullOrEmpty(this string target)
        {
            return string.IsNullOrEmpty(target);
        }

        public static bool IsEmpty(this string target)
        {
            if (target == null) return false;
            if (target.Trim() == null) return true;
            return false;
        }

        public static bool IsNull(string target)
        {
            return (target == null);
        }

        public static string FormatWith(this string target, params object[] args)
        {
            if (IsNullOrEmpty(target)) return string.Empty;
            return string.Format(CultureInfo.CurrentCulture, target, args);
        }

    }
}