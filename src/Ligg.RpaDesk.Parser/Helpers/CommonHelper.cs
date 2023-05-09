
using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using  Ligg.Infrastructure.Utilities.DataParserUtil;


namespace Ligg.RpaDesk.Parser.Helpers
{
    public static class CommonHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //*set
        public static void SetApplicationAnnexes(List<Annex> annexes, string appCode)
        {
            foreach (var annex in annexes)
            {
                annex.MasterName = appCode;
                annex.ClassName = "Application";
            }
        }
        public static void SetFormTitleAnnexes(List<Annex> annexes, string funcCode)
        {
            foreach (var annex in annexes)
            {
                annex.MasterName = funcCode;
                annex.ClassName = "FormTitle";
            }
        }

        //*get
        //public static T GetGenericFromCfgFile<T>(string path, bool neccesary)
        //{
        //    return ConfigFileUtilHelper.GetGenericFromCfgFile<T>(path, neccesary);
        //}

        public static List<Annex> GetAnnexesFromCfgFile(string path, string class1, bool neccesary)
        {
            var annexes = new List<Annex>();
            var annexList = DataParserHelper.ConvertToGeneric<List<Annex>>(path, neccesary, TxtDataType.Undefined);
            if (annexList != null)
                if (annexList.Count > 0)
                {
                    foreach (var annex in annexList)
                    {
                        annex.ClassName = class1;
                        annexes.Add(annex);
                    }
                }
            return annexes;
        }

        public static string GetDisplayName(bool supportMultiLangs, string className, string masterName, List<Annex> annexes, string defText)
        {
            return GetAnnexText(supportMultiLangs, AnnexTextType.DisplayName, className, masterName, annexes, defText);
        }

        private static string GetAnnexText(bool supportMultiLangs, AnnexTextType annexTextType, string className, string masterName, List<Annex> annexes, string defText)
        {

            if (supportMultiLangs)
            {
                var text = AnnexHelper.GetText(className, masterName, annexes, annexTextType, GetAnnexMode.Current, defText);
                return text;
            }
            else return defText.IsNullOrEmpty() ? "" : defText;
        }

        //*judge
        //public static bool IsConfigFileExisting(string filePath)
        //{
        //    return ConfigFileUtilHelper.IsConfigFileExisting(filePath);
        //}

        //*check
        public static void CheckName(string text)
        {
            var exInfo = "\n>> " + _typeFullName + ".CheckName Error: ";
            if (text.IsNullOrEmpty())
            {
                throw new ArgumentException(exInfo + "UiElememt Name= " + text + " can not be empty! ");
            }
            if (!StringExtension.AlphaAndNumeralExpression.IsMatch(text))
            {
                throw new ArgumentException(exInfo + "UiElememt Name =" + text + "is not in valid format, UiElememt Name can only includes alpha,numeral! ");
            }
        }



    }
}