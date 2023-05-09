using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using  Ligg.Infrastructure.DataModels;


namespace Ligg.RpaDesk.Parser.Helpers
{
    public static class IdentifierHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static readonly string ShellIdentifer = "#";
        public static readonly string UiIdentifer = "$";
        public static readonly string LrunIdentifer = "{}";
        private static readonly char[] _formularParamSeparators = new char[] { '^', '@', ';' };//&--xml不支持  XmlHelper.ConvertToGeneric Error: XML 文档(41, 126)中有错误。
        public static readonly string[] CentralDataExchangeDataSeparators = new string[] { "{{", "}}" };

        private static readonly List<ValueText> OptionalIdentifers = new List<ValueText>() {
            new ValueText(){Value="$",Text="§"},
            new ValueText(){Value="#",Text="＃"},
            new ValueText(){Value="~",Text="～"},
            new ValueText(){Value="{}",Text="｛｝"},
            new ValueText(){Value="^",Text="＾"},new ValueText(){Value="@",Text="＠"},new ValueText(){Value=";",Text="；"},new ValueText(){Value="|",Text="｜"},
            new ValueText(){Value="/",Text="／"},new ValueText(){Value=",",Text="，"},
            new ValueText(){Value="*",Text="★"},new ValueText(){Value=":",Text="∶"},new ValueText(){Value="=",Text="★"},
            new ValueText(){Value="[",Text="［"},new ValueText(){Value="]",Text="］"},
            new ValueText(){Value="[[",Text="［［"},new ValueText(){Value="]]",Text="］］"},
        };



        //shell
        public static string AddShellIdentifer(this string target)
        {
            return ShellIdentifer + target + ShellIdentifer;
        }
        public static string DeleteShellIdentifer(this string target)
        {
            CheckProcessElementDataFormat(target);
            var str = target;
            if (str.Contains(ShellIdentifer))
            {
                str = str.Replace(ShellIdentifer, "");
            }
            return str;
        }


        public static bool IsShellElementDataFormat(this string text)
        {

            if (text.IsNullOrEmpty()) return false;
            if (!text.EndsWith(ShellIdentifer) | !text.StartsWith(ShellIdentifer)) return false;
            if (text.GetQtyOfIncludedChar(ShellIdentifer.ToChar()) != 2) return false;

            return true;
        }

        public static bool ContainsShellIdentifer(string str)
        {
            if (str.Contains(ShellIdentifer))
            {
                return true;
            }
            return false;
        }

        public static void CheckProcessElementDataFormat(this string str)
        {
            var exInfo = "\n>> " + _typeFullName + ".CheckProcessElement Error: ";
            if (!IsShellElementDataFormat(str)) throw new ArgumentException(exInfo + "str must be in  Shell Element format, ! that's to say, should start with and end with #, str= " + str);
        }

        //*ui
        public static string AddUiIdentifer(this string target)
        {
            return UiIdentifer + target + UiIdentifer;

        }
        public static string DeleteUiIdentifer(this string target)
        {
            CheckUiElementDataFormat(target);
            var str = target;
            if (str.Contains(UiIdentifer))
            {
                str = str.Replace(UiIdentifer, "");
            }
            return str;
        }

        public static bool IsUiElementDataFormat(this string text)
        {

            if (text.IsNullOrEmpty()) return false;
            if (!text.EndsWith(UiIdentifer) | !text.StartsWith(UiIdentifer)) return false;
            if (text.GetQtyOfIncludedChar(UiIdentifer.ToChar()) != 2) return false;

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
        public static void CheckUiElementDataFormat(this string str)
        {
            var exInfo = "\n>> " + _typeFullName + ".CheckUiElement Error: ";
            if (!IsUiElementDataFormat(str)) throw new ArgumentException(exInfo + "Str must be in UiElement format! that's to say, should start with and end with $, str= " + str);
        }

        //*ced
        public static bool IsCenterExchangeDataFormat(this string text)
        {

            if (text.IsNullOrEmpty()) return false;
            if (!text.StartsWith(CentralDataExchangeDataSeparators[0]) | !text.EndsWith(CentralDataExchangeDataSeparators[1])) return false;
            if (text.GetQtyOfIncludedString(CentralDataExchangeDataSeparators[0]) != 1) return false;
            if (text.GetQtyOfIncludedString(CentralDataExchangeDataSeparators[1]) != 1) return false;
            return true;
        }

        public static void CheckCenterExchangeDataFormat(this string str)
        {
            var exInfo = "\n>> " + _typeFullName + ".CheckCenterDataExchangeDataFormat Error: ";
            if (!IsCenterExchangeDataFormat(str)) throw new ArgumentException(exInfo + "Str must be in CenterDataExchangeData format! that's to say, should start with [ and end with ], str= " + str);
        }

        //*formularParam
        public static string UnwrapFormularParamArray(this String[] strArry)
        {
            var exInfo = "\n>> " + _typeFullName + ".UnwrapParamArray Error: ";
            string separator = _formularParamSeparators[0].ToString();
            if (strArry.Any(x => x==null)) throw new ArgumentException(exInfo + "any of strArry should not be null! ");
            if (strArry.Any(x => x.Contains(_formularParamSeparators[0]))) throw new ArgumentException(exInfo + "any of strArry should not contain highest level Separator: " + _formularParamSeparators[0]);

            if (strArry.Any(x => !x.Contains(_formularParamSeparators[2]))) separator = _formularParamSeparators[2].ToString();
            else if (strArry.Any(x => !x.Contains(_formularParamSeparators[1]))) separator = _formularParamSeparators[1].ToString();
            else separator = _formularParamSeparators[0].ToString();
            return strArry.Unwrap(separator);
        }

        public static char GeFormularParamSeparator(this string target)
        {
            var separator = _formularParamSeparators[2];
            if (target.Contains(_formularParamSeparators[0])) separator = _formularParamSeparators[0];
            else if (target.Contains(_formularParamSeparators[1])) separator = _formularParamSeparators[1];
            return separator;
        }

        public static string[] GetFormularParamArray(this string target, bool trim, bool clear)
        {
            var separator = target.GeFormularParamSeparator();
            var arry = target.Split(separator);

            if (trim) arry = StringArrayExtension.Trim(arry);
            if (clear) arry = StringArrayExtension.Clear(arry);
            return arry;
        }

        public static bool ContainsFormularParamSeparator(this string target)
        {
            foreach (var separator in _formularParamSeparators)
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

        ////*reservedChar
        //public static string RecoverReservedIdentifers(this string target, string tabooIdentifers)
        //{
        //    if (target.IsNullOrEmpty()) return string.Empty;
        //    if (tabooIdentifers.IsNullOrEmpty()) return target;
        //    var valTxts = OptionalIdentifers;
        //    if (tabooIdentifers.ToLower() != "a" & tabooIdentifers.ToLower() != "all")
        //    {
        //        var vals = tabooIdentifers.GetLarrayArray(true, true);
        //        valTxts = OptionalIdentifers.FindAll(x => vals.Contains(x.Value));
        //    }

        //    foreach (var valTxt in valTxts)
        //    {
        //        if ((target.Contains(valTxt.Text)))
        //        {
        //            target = target.Replace(valTxt.Text, valTxt.Value);
        //        }
        //    }

        //    return target;
        //}

        public static string RepalceReservedIdentifers(this string target, string ids) //clean RTI
        {
            if (target.IsNullOrEmpty()) return string.Empty;


            var idsArry = new string[] { };
            if (!(ids.ToLower() == "all" | ids.ToLower() == "a" | ids.IsNullOrEmpty()))
                idsArry = ids.SplitByChar(' ', true, true);

            foreach (var valTxt in OptionalIdentifers)
            {
                if ((target.Contains(valTxt.Value)))
                {
                    if ((idsArry.Length > 0 & idsArry.Contains("valTxt.Value")) | idsArry == null)
                        target = target.Replace(valTxt.Value, valTxt.Text);
                }
            }

            return target;
        }


    }


}
