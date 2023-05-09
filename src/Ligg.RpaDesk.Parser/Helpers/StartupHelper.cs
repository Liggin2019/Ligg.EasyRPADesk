
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.RpaDesk.Parser.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Ligg.RpaDesk.Parser.Helpers
{
    public static class StartupHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static T GetApplicationSetting<T>(string architectureCode, string appCode) where T : ApplicationSetting
        {
            var exInfo = _typeFullName + ".GetApplicationSetting Error: ";
            var executableDir = Path.GetDirectoryName(PathHelper.GetMainModuleFileName());
            var startupDir = Directory.GetParent(executableDir).ToString();
            startupDir = Directory.GetParent(startupDir).ToString();
            startupDir = Directory.GetParent(startupDir).ToString();
            Directory.SetCurrentDirectory(startupDir);

            var cfgDir = startupDir + "\\Conf";
            var cfgFile = cfgDir + "\\GlobalSetting";
            var globalSetting = DataParserHelper.ConvertToGeneric<GlobalSetting>(cfgFile, true, TxtDataType.Undefined);

            LanguageHelper.CheckCultureNameLegality(globalSetting.DefaultCulture);
            if (!globalSetting.HelpdeskEmail.IsLegalEmailAddress()) throw new ArgumentException(exInfo + "HelpdeskEmail can't be empty and format should be correct in config file: " + cfgFile);

            var appCfgDir = cfgDir + "\\Apps\\" + appCode;
            cfgFile = appCfgDir + "\\ApplicationSetting";
            var appSetting = DataParserHelper.ConvertToGeneric<T>(cfgFile, true, TxtDataType.Undefined);
            if (appSetting.DefaultLanguageCode.IsNullOrEmpty()) throw new ArgumentException(exInfo + "DefaultLanguageCode can't be empty in config file: " + cfgFile);

            var appDataDir = "";
            if (appSetting.ApplicationDataDir.IsNullOrEmpty())
            {
                appDataDir = startupDir + "\\Data\\Apps\\" + appCode;
            }
            else
            {
                appDataDir = FileHelper.GetPath(appSetting.ApplicationDataDir, startupDir + "\\Data" + appCode);
            }
            appSetting.ApplicationDataDir = appDataDir;

            var appLibDir = "";
            if (appSetting.ApplicationLibDir.IsNullOrEmpty())
            {
                appLibDir = startupDir + "\\Lib\\Apps\\" + appCode;
            }
            else
            {
                appLibDir = FileHelper.GetPath(appSetting.ApplicationLibDir, startupDir + "\\Lib" + appCode);
            }
            appSetting.ApplicationLibDir = appLibDir;

            var appOutputDir = "";
            if (appSetting.ApplicationOutputDir.IsNullOrEmpty())
            {
                appOutputDir = PathHelper.GetLastDriveName() + "\\tmp\\" + architectureCode + "\\" + appCode;
            }
            else
            {
                appOutputDir = FileHelper.GetPath(appSetting.ApplicationOutputDir, startupDir + "\\Data\\" + appCode + "\\Tmp");
            }
            SysProcessHelper.TmpPath = appOutputDir;
            appSetting.ApplicationOutputDir = appOutputDir;


            if (!globalSetting.PhythonExePath.IsNullOrEmpty())
            {
                var defLoc = Directory.GetParent(appLibDir).ToString();
                defLoc = Directory.GetParent(defLoc).ToString();
                var phythonExePath = FileHelper.GetPath(globalSetting.PhythonExePath, defLoc);
                appSetting.PhythonExecPath = phythonExePath;
                SysProcessHelper.PythonExecPath = phythonExePath;
            }

            if (appSetting.DefaultCulture.IsNullOrEmpty()) appSetting.DefaultCulture = globalSetting.DefaultCulture;
            if (appSetting.HelpdeskEmail.IsNullOrEmpty()) appSetting.HelpdeskEmail = globalSetting.HelpdeskEmail;
            return appSetting;
        }

        public static T GetFormStartPolicy<T>(string appCode, FormType formType, string formRelativePath) where T : FormStartPolicy
        {
            var exInfo = "\n>> " + _typeFullName + ".GetFormStartPolicy Error: ";

            var curDir = Directory.GetCurrentDirectory();
            var cfgDir = curDir + "\\Conf";
            var appCfgDir = cfgDir + "\\Apps\\" + appCode;
            string formCFgDir = "";
            if (formType == FormType.Snr)
            {
                formCFgDir = appCfgDir + "\\Ui\\Console\\Scenarios\\" + formRelativePath;
            }
            else if (formType == FormType.Mvi)
            {
                formCFgDir = appCfgDir + "\\Ui\\WinForm\\Portals\\" + formRelativePath;
            }
            else if (formType == FormType.Svi)
            {
                formCFgDir = appCfgDir + "\\Ui\\WinForm\\Views\\" + formRelativePath;
            }
            else
            {
                formCFgDir = appCfgDir + "\\Ui\\WinForm\\Zones\\" + formRelativePath;
            }
            if (!DirectoryHelper.IsDirectoryExisting(formCFgDir)) throw new ArgumentException(exInfo + String.Format("Form config folder: {0} doesn't exist! ", formCFgDir));


            var cfgFile = formCFgDir + "\\StartPolicy";

            var startPolicy = DataParserHelper.ConvertToGeneric<T>(cfgFile, false, TxtDataType.Undefined);

            return startPolicy;

        }

        //*doing
        public static void SetLanguage(string langId, string includedLanguages)
        {
            var exInfo = _typeFullName + ".SetLanguage Error: ";
            var curDir = Directory.GetCurrentDirectory();
            var cfgDir = curDir + "\\Conf";
            var cfgFile = cfgDir + "\\Languages\\Languages";

            var langs = new List<Language>();
            try
            {
                langs = DataParserHelper.ConvertToGeneric<List<Language>>(cfgFile, true, TxtDataType.Undefined);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("SetLanguage Error: " + ex.Message);
            }
            if (langs.Count == 0) throw new ArgumentException(exInfo + "Languages can't be null!");

            var langIdsArry = includedLanguages.SplitByChar(',', true, true);
            var languages = new List<Language>();

            foreach (var lang in langs)
            {
                LanguageHelper.CheckCultureNameLegality(lang.CultureName);
                var id = lang.CultureName.Trim() + ":" + lang.LanguageCode.Trim();
                lang.Id = id;
                lang.ImageUrl = FileHelper.GetPath(lang.ImageUrl, cfgDir + "\\Languages");
                if (!includedLanguages.IsNullOrEmpty())
                {
                    if (langIdsArry.Length > 0)
                    {
                        if (langIdsArry.Contains(id))
                        {
                            languages.Add(lang);
                        }
                    }
                }
                else
                {
                    languages.Add(lang);
                }
            }

            LanguageHelper.Languages = languages;
            foreach (var lang in languages)
            {
                if (lang.LanguageCode.IsNullOrEmpty()) throw new ArgumentException(exInfo + "LanguageCode can't be empty, Language: " + lang.Id);
                LanguageHelper.CheckLanguageIdValidity(langId);
            }

            var language = new Language();
            //var osCultureName = LanguageHelper.GetOsCultureName();
            //var osLanguage = languages.Find(x => x.CultureName == osCultureName);
            //if (osLanguage != null)
            //{
            //    language = osLanguage;
            //}
            //else 
            language = languages.Find(x => x.Id == langId);
            LanguageHelper.SetCulture(language.CultureName);
            language.IsCurrent = true;
            language.IsDefault = true;
            AnnexHelper.CurrentLanguageCode = language.LanguageCode;
            AnnexHelper.DefaultLanguageCode = language.LanguageCode;
        }



    }
}