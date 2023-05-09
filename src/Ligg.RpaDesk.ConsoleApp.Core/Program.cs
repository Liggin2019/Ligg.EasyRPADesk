
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.RpaDesk.Interface;
using Ligg.RpaDesk.Parser.DataModels;
using Ligg.RpaDesk.Parser.Helpers;
using Ligg.RpaDesk.Resources;
using Ligg.RpaDesk.WinCnsl.DataModels;
using System;
using System.Diagnostics;
using System.IO;

namespace Ligg.RpaDesk
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var startupDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
#if DEBUG
            if (args.Length == 0)
            {

                var debugIniPath = startupDir + "\\Debug.ini";
                if (File.Exists(debugIniPath))
                {
                    string debugIniArgsStr = IniFileHelper.ReadIniString(debugIniPath, "setting", "args", "");
                    args = debugIniArgsStr.Split(' ');
                }
            }
#endif
            if (args.Length < 2) goto End;
            var startArgs = new StartArgs();
            //by liggin2019 on 20200405
            //appArgsStr format
            //appCode^isRedirectedFlag
			
            var startArgStrArry = args[0].GetFormularParamArray(true, false);
            startArgs.AppCode = startArgStrArry[0];
            startArgs.IsRedirectedFlag = startArgStrArry.Length > 1 ? startArgStrArry[1] : "";

            if (args.Length > 2) startArgs.StartLanguage = args[2];

            try
            {
                
                var startup = new Startup(startArgs.AppCode);
                var appSetting = startup.ApplicationSetting;
                if (appSetting.SupportMultiLanguages)
                {
                    var startCultureId = startArgs.StartLanguage.IsNullOrEmpty() ? appSetting.DefaultCulture.Trim() + ":" + appSetting.DefaultLanguageCode.Trim() : startArgs.StartLanguage;
                    StartupHelper.SetLanguage(startCultureId, appSetting.IncludedLanguages);
                }
                else
                {
                    LanguageHelper.SetCulture(appSetting.DefaultCulture);
                    AnnexHelper.CurrentLanguageCode = appSetting.DefaultLanguageCode;
                    AnnexHelper.DefaultLanguageCode = appSetting.DefaultLanguageCode;
                }
                CentralData.InitApplicationSetting(startArgs.AppCode, appSetting.SupportMultiLanguages, LanguageHelper.DefaultLanguageCode, appSetting.ApplicationLibDir, appSetting.ApplicationDataDir, appSetting.ApplicationOutputDir, appSetting.PhythonExecPath);
                Bootstrapper.Initialize(appSetting, startArgs.AppCode);

                var formArgsStr = args[1];
                var formArgsArry = formArgsStr.GetFormularParamArray(true, false);
                if (formArgsArry.Length < 1) goto End;

                //by liggin2019 on 20200405
                //formArgsStr format
                //formRelativeLocation^formTitle
                startArgs.FormRelativeLocation = formArgsArry[0];
                startArgs.FormTitle = formArgsArry.Length > 1 ? formArgsArry[1] : "";

                if (DirectoryHelper.IsLegalAbsoluteDirectory(startArgs.FormRelativeLocation)) throw new ArgumentException("startArgs.FormRelativeLocation=" + startArgs.FormRelativeLocation + " can't be an absolute path! ");
                if (startArgs.FormRelativeLocation.StartsWith("~")) throw new ArgumentException("formLocation can't contain \"~\"! ");
                if (startArgs.FormRelativeLocation.StartsWith("\\")) startArgs.FormRelativeLocation = startArgs.FormRelativeLocation.Substring(1, startArgs.FormRelativeLocation.Length - 1);
                if (startArgs.FormRelativeLocation.EndsWith("\\")) startArgs.FormRelativeLocation = startArgs.FormRelativeLocation.Substring(0, startArgs.FormRelativeLocation.Length - 2);
                if (startArgs.FormRelativeLocation.StartsWith("\\") | startArgs.FormRelativeLocation.StartsWith("\\")) throw new ArgumentException("formLocation is incorrect! ");
                {
                    var functionCode = "";
                    if (startArgs.FormRelativeLocation.Contains("\\"))
                    {
                        var tmpArry = startArgs.FormRelativeLocation.Split('\\');
                        functionCode = startArgs.FormRelativeLocation.EndsWith("\\") ? tmpArry[tmpArry.Length - 2] : tmpArry[tmpArry.Length - 1];
                    }
                    else
                    {
                        functionCode = startArgs.FormRelativeLocation;
                    }

                    if (!StringExtension.AlphaNumeralAndHyphenExpression.IsMatch(functionCode)) throw new ArgumentException("FormLocation last level folder can only includes alpha, numeral, hyphen! ");
                }

                startup.SetApplicationStartPolicy(startArgs.AppCode, FormType.Snr, startArgs.FormRelativeLocation);
                ConsoleHelper.CloseQuickEditMode();

                //#subsequent actions by formArgStr
                //##set formInitParamSet
                var formInitParamSet = new FormInitParamSet();
                formInitParamSet.ArchitectureCode = CentralData.ArchitectureCode;
                formInitParamSet.ArchitectureName = CentralData.ArchitectureName;
                formInitParamSet.ArchitectureVersion = CentralData.ArchitectureVersion;
                formInitParamSet.OrganizationCode = CentralData.OrganizationCode;
                formInitParamSet.OrganizationShortName = CentralData.OrganizationShortName;
                formInitParamSet.OrganizationName = CentralData.OrganizationName;
                formInitParamSet.ApplicationCode = startArgs.AppCode;
                formInitParamSet.ApplicationVersion = appSetting.ApplicationVersion;
                formInitParamSet.FormRelativeLocation = startArgs.FormRelativeLocation;
                formInitParamSet.FormTitle = startArgs.FormTitle ?? "";
                formInitParamSet.HelpdeskEmail = appSetting.HelpdeskEmail;
                formInitParamSet.SupportMultiLanguages = appSetting.SupportMultiLanguages;

                formInitParamSet.ApplicationLibDir = appSetting.ApplicationLibDir;
                formInitParamSet.ApplicationDataDir = appSetting.ApplicationDataDir;
                formInitParamSet.ApplicationOutputDir = appSetting.ApplicationOutputDir;

                //##ShowSoftwareCover
                if (appSetting.ShowSoftwareCoverAtStart)
                {
                    bool showSoftwareCover = startArgs.IsRedirectedFlag.ToLower() != "true";
                    if (showSoftwareCover)
                    {
                        var isOk = startup.RunScenario(formInitParamSet, true);
                        if (!isOk) goto End;
                    }
                }

                var logon = appSetting.LogonAtStart;

                //##Logon
                if (logon)
                {
                    if (logon) if (!startup.RunScenario(formInitParamSet, false)) goto End;
                }

                var form = new StartForm(formInitParamSet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(TextRes.PressAnyKeyToExit + " !");
                Console.ReadKey();
                ConsoleHelper.CloseConsole();
            }
        End:;

        }

    }

}
