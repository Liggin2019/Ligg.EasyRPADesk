using System;
using System.Text.RegularExpressions;
using Ligg.Infrastructure.Helpers;

namespace Ligg.RpaDesk.Parser.Helpers
{
    public static class Resolver
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;


        //*resolve
        public static string ResolveConstants(string text)
        {
            if (!text.Contains("%")) return text;

            var toBeRplStr = "";
            toBeRplStr = "%Now%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%UtcNow%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = SystemTimeHelper.UtcNow().ToString("yyyy-MM-dd HH:mm:ss");
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%r%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = "\r";
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%n%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = "\n";
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%t%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = "\t";
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            return GetHelper.ResolveSpecialFolder(text);
        }



    }
}