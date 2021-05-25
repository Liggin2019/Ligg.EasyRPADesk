using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Base.DataModel;


namespace Ligg.EasyWinApp.Parser.Helpers
{
    public static class IdentifierHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static readonly string ProcessIdentifer = "#";
        public static readonly string UiIdentifer = "$";
        public static readonly string LrunIdentifer = "{}";
        private static readonly char[] ParamSeparators = new char[] { '^', '@', ';' };//&--xml不支持
        private static readonly char[] SubParamSeparators = new char[] { '|', '/', ',' };
        private static readonly List<ValueText> TabooIdentifers = new List<ValueText>() {
            new ValueText(){Value="#",Text="＃"},new ValueText(){Value="$",Text="§"},new ValueText(){Value="#",Text="＃"},
            new ValueText(){Value="~",Text="～"},new ValueText(){Value="{}",Text="｛｝"},
            new ValueText(){Value="^",Text="＾"},new ValueText(){Value="@",Text="＠"},new ValueText(){Value=";",Text="；"},
            new ValueText(){Value="|",Text="｜"},new ValueText(){Value="/",Text="／"},new ValueText(){Value=",",Text="，"},
        };

        //*taboochars
        public static string RecoverTabooIdentifers(this string target, string tabooIdentifers)
        {
            if (target.IsNullOrEmpty()) return string.Empty;
            if (tabooIdentifers.IsNullOrEmpty()) return target;
            var valTxts = TabooIdentifers;
            if (tabooIdentifers.ToLower() != "a" & tabooIdentifers.ToLower() != "all")
            {
                var vals = tabooIdentifers.GetSubParamArray(true, true);
                valTxts = TabooIdentifers.FindAll(x => vals.Contains(x.Value));
            }

            foreach (var valTxt in valTxts)
            {
                if ((target.Contains(valTxt.Text)))
                {
                    target = target.Replace(valTxt.Text, valTxt.Value);
                }
            }

            return target;
        }

        public static string RepalceTabooIdentifers(this string target) //clean RTI
        {
            if (target.IsNullOrEmpty()) return string.Empty;
            foreach (var valTxt in TabooIdentifers)
            {
                if ((target.Contains(valTxt.Value)))
                {
                    target = target.Replace(valTxt.Value, valTxt.Text);
                }
            }

            return target;
        }

        //*process
        public static string AddProcessIdentifer(this string target)
        {
            return ProcessIdentifer + target + ProcessIdentifer;
        }
        public static string DeleteProcessIdentifer(this string target)
        {
            CheckProcessElement(target);
            var str = target;
            if (str.Contains(ProcessIdentifer))
            {
                str = str.Replace(ProcessIdentifer, "");
            }
            return str;
        }

        public static bool IsInputProcessParam(string text)
        {

            if (text.IsNullOrEmpty()) return false;
            if (!text.EndsWith(ProcessIdentifer) | !text.StartsWith(ProcessIdentifer)) return false;
            var strArr = text.Split(ProcessIdentifer.ToChar());

            var inputNo = strArr[1];
            if (inputNo.IsPlusIntegerOrZero())
            {
                return true;
            }

            return false;
        }

        public static bool IsProcessElement(string text)
        {

            if (text.IsNullOrEmpty()) return false;
            if (!text.EndsWith(ProcessIdentifer) | !text.StartsWith(ProcessIdentifer)) return false;
            var strArr = text.Split(ProcessIdentifer.ToChar());

            var inputNo = strArr[1];
            if (inputNo.IsPlusIntegerOrZero())
            {
                return false;
            }

            return true;
        }

        public static bool ContainsProcessIdentifer(string str)
        {
            if (str.Contains(ProcessIdentifer))
            {
                return true;
            }
            return false;
        }

        private static void CheckProcessElement(string str)
        {
            var exInfo = "\n>> " + TypeName + ".CheckProcessElement Error: ";
            if (!IsProcessElement(str)) throw new ArgumentException(exInfo+ "Str must be in  ProcessElement format, ! that's to say, should start with and end with #, str= " + str);
        }

        //*ui
        public static string AddUiIdentifer(this string target)
        {
            return UiIdentifer + target + UiIdentifer;

        }
        public static string DeleteUiIdentifer(this string target)
        {
            CheckUiElement(target);
            var str = target;
            if (str.Contains(UiIdentifer))
            {
                str = str.Replace(UiIdentifer, "");
            }
            return str;
        }

        public static bool IsUiElement(string text)
        {

            if (text.IsNullOrEmpty()) return false;
            if (!text.EndsWith(UiIdentifer) | !text.StartsWith(UiIdentifer)) return false;
            var strArr = text.Split(UiIdentifer.ToChar());

            var inputNo = strArr[1];
            if (inputNo.IsPlusIntegerOrZero())
            {
                return false;
            }

            return true;
        }
        public static bool ContainsUiIdentifer(string str)
        {
            if (str.Contains(UiIdentifer))
            {
                return true;
            }
            return false;
        }
        private static void CheckUiElement(string str)
        {
            var exInfo = "\n>> " + TypeName + ".CheckUiElement Error: ";
            if (!IsUiElement(str)) throw new ArgumentException(exInfo+"Str must be in UiElement format! that's to say, should start with and end with $, str= "+ str);
        }


        //*param
        public static string UnwrapParamArray(String[] strArry)
        {
            var exInfo = "\n>> " + TypeName + ".UnwrapParamArray Error: ";
            char separator = ParamSeparators[0];
            if (strArry.Any(x => x.Contains(ParamSeparators[0]))) throw new ArgumentException(exInfo + "strArry should not contain ParamSeparators[0]: " + ParamSeparators[0]);

            if (strArry.Any(x => !x.Contains(ParamSeparators[2]) & !x.Contains(ParamSeparators[1]) & !x.Contains(ParamSeparators[0]))) separator = ParamSeparators[2];
            else if (strArry.Any(x => x.Contains(ParamSeparators[2]) & (!x.Contains(ParamSeparators[1]) & !x.Contains(ParamSeparators[0])))) separator = ParamSeparators[1];
            else if (strArry.Any(x => x.Contains(ParamSeparators[1]) & (!x.Contains(ParamSeparators[2]) & !x.Contains(ParamSeparators[0])))) separator = ParamSeparators[0];

            return StringHelper.UnwrapStringArrayBySeparator(strArry, separator);
        }

        public static char GetParamSeparator(this string target)
        {
            var separator = ParamSeparators[2];
            if (target.Contains(ParamSeparators[0])) separator = ParamSeparators[0];
            else if (target.Contains(ParamSeparators[1])) separator = ParamSeparators[1];
            return separator;
        }

        public static string[] GetParamArray(this string target, bool trim, bool clear)
        {
            var separator = target.GetParamSeparator();
            var arry = target.Split(separator);

            if (trim) arry = StringHelper.TrimArray(arry);
            if (clear) arry = StringHelper.ClearArray(arry);
            return arry;
        }

        public static bool ContainsParamSeparator(this string target)
        {
            foreach (var separator in ParamSeparators)
            {
                if (target.Contains(separator)) return true;
            }
            return false;
        }


        //*subparam
        public static string UnwrapSubParamArray(String[] strArry)
        {
            var exInfo = "\n>> " + TypeName + ".UnwrapSubParamArray Error: ";
            char separator = SubParamSeparators[0];
            if (strArry.Any(x => x.Contains(SubParamSeparators[0]))) throw new ArgumentException(exInfo + "strArry should not contain SubParamSeparators[0]: " + SubParamSeparators[0]);

            if (strArry.Any(x => !x.Contains(SubParamSeparators[2])& !x.Contains(SubParamSeparators[1])&!x.Contains(SubParamSeparators[0]))) separator = SubParamSeparators[2];
            else if (strArry.Any(x => x.Contains(SubParamSeparators[2])&(!x.Contains(SubParamSeparators[1]) & !x.Contains(SubParamSeparators[0])))) separator = SubParamSeparators[1];
            else if (strArry.Any(x => x.Contains(SubParamSeparators[1]) & (!x.Contains(SubParamSeparators[2]) & !x.Contains(SubParamSeparators[0])))) separator = SubParamSeparators[0];
            return StringHelper.UnwrapStringArrayBySeparator(strArry, separator);
        }

        public static char GetSubParamSeparator(this string target)
        {
            var separator = SubParamSeparators[2];
            if (target.Contains(SubParamSeparators[0])) separator = SubParamSeparators[0];
            else if (target.Contains(SubParamSeparators[1])) separator = SubParamSeparators[1];
            return separator;
        }

        public static string[] GetSubParamArray(this string target, bool trim, bool clear)
        {
            var separator = target.GetSubParamSeparator();
            var arry = target.Split(separator);

            if (trim) arry = StringHelper.TrimArray(arry);
            if (clear) arry = StringHelper.ClearArray(arry);
            return arry;
        }



        public static bool ContainsSubParamSeparator(this string target)
        {
            foreach (var separator in SubParamSeparators)
            {
                if (target.Contains(separator)) return true;
            }
            return false;
        }

        //*lrun
        public static bool ContainsLrunIdentifer(string str)
        {
            if (str.Contains(LrunIdentifer))
            {
                return true;
            }
            return false;
        }


    }


}
