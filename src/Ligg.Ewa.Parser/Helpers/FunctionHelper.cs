using System.Collections.Generic;
using Ligg.Infrastructure.Base.DataModel;
using Ligg.Infrastructure.Base.DataModel.Enums;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;

using Ligg.Infrastructure.Utility.FileWrap;

namespace Ligg.EasyWinApp.Parser.Helpers
{
    public static class FunctionHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //#form
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
                annex.ClassName = "Function";
            }
        }

        public static T GetGenericFromCfgFile<T>(string path, bool neccesary)
        {
            if (neccesary | ConfigFileHelper.IsFileExisting(path))
            {
                var cfgFileMgr = new ConfigFileManager(path);
                return cfgFileMgr.ConvertToGeneric<T>();
            }
            return default(T);
        }

        public static List<Annex> GetAnnexesFromCfgFile(string path, string kind, bool neccesary)
        {
            var annexes = new List<Annex>();
            if (neccesary | ConfigFileHelper.IsFileExisting(path))
            {
                var cfgFileMgr = new ConfigFileManager(path);
                var annexList = cfgFileMgr.ConvertToGeneric<List<Annex>>();
                if (annexList.Count > 0)
                {
                    foreach (var annex in annexList)
                    {
                        annex.ClassName = kind;
                        annexes.Add(annex);
                    }
                }
            }
            return annexes;
        }

        public static string GetDisplayName(bool supportMultiLangs, string className, string ctrlName, List<Annex> annexes, string defText)
        {
            return GetAnnexText(supportMultiLangs, AnnexTextType.DisplayName, className, ctrlName, annexes, defText);
        }

        public static string GetDescription(bool supportMultiLangs, string className, string ctrlName, List<Annex> annexes, string defText)
        {
            return GetAnnexText(supportMultiLangs, AnnexTextType.Remark, className, ctrlName, annexes, defText);
        }

        public static string GetRemark(bool supportMultiLangs, string className, string ctrlName, List<Annex> annexes, string defText)
        {
            return GetAnnexText(supportMultiLangs, AnnexTextType.Remark, className, ctrlName, annexes, defText);
        }
        public static string GetRemark1(bool supportMultiLangs, string className, string ctrlName, List<Annex> annexes, string defText)
        {
            return GetAnnexText(supportMultiLangs, AnnexTextType.Remark1, className, ctrlName, annexes, defText);
        }

        public static string GetRemark2(bool supportMultiLangs, string className, string ctrlName, List<Annex> annexes, string defText)
        {
            return GetAnnexText(supportMultiLangs, AnnexTextType.Remark2, className, ctrlName, annexes, defText);
        }


        private static string GetAnnexText(bool supportMultiLangs, AnnexTextType annexTextType, string className, string ctrlName, List<Annex> annexes, string defText)
        {

            if (supportMultiLangs)
            {
                var text = AnnexHelper.GetText(className, ctrlName, annexes, annexTextType, CultureHelper.CurrentLanguageCode, GetAnnexMode.OnlyByCurLang);
                if (text.IsNullOrEmpty()) text = defText;
                return text;
            }
            else return defText;

        }

        public static string GetJudgementFlag(this string target)
        {
            if (target.IsNullOrEmpty()) return "false";
            if (target.ToLower() == "true") return "true";
            return "false";
        }


    }
}