using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Ligg.Infrastructure.Extensions
{
    internal static partial class StringExtension
    {
        //*regex
        internal static readonly Regex AlphaNumeralUnderscoreHyphenAndChineseExpression = new Regex("^[A-Za-z0-9_\\-\u4e00-\u9fa5]+$");
        internal static readonly Regex AlphaAndNumeralExpression = new Regex("^[A-Za-z0-9]+$");
        internal static readonly Regex AlphaNumeralAndHyphenExpression = new Regex("^[A-Za-z0-9\\-]+$");
        internal static readonly Regex AlphaNumeralHyphenAndDotExpression = new Regex("^[A-Za-z0-9\\-\\.]+$");
        internal static readonly Regex ChineseExpression = new Regex("^[\u4e00-\u9fa5]+$ ");
        internal static readonly Regex EnglishExpression = new Regex(@"^[A-Za-z]+$");
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

        internal static bool IsNullOrEmpty(this string target)
        {
            return string.IsNullOrEmpty(target);
        }

        internal static bool IsEmpty(this string target)
        {
            if (target == null) return false;
            if (target.Trim() == null) return true;
            return false;
        }

        internal static bool IsNull(string target)
        {
            return (target == null);
        }

        internal static string FormatWith(this string target, params object[] args)
        {
            if (IsNullOrEmpty(target)) return string.Empty;
            return string.Format(CultureInfo.CurrentCulture, target, args);
        }

    }
}