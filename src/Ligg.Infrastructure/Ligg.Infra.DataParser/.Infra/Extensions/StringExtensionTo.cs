using Ligg.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Ligg.Infrastructure.Extensions
{
    internal static partial class StringExtension
    {

        internal static string ToLegalFileName(this string target)
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


 
    }
}