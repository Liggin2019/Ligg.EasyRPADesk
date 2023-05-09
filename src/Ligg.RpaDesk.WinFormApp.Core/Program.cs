using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.RpaDesk.Interface;
using Ligg.RpaDesk.Parser.DataModels;
using Ligg.RpaDesk.Parser.Helpers;
using Ligg.RpaDesk.Resources;
using Ligg.RpaDesk.WinForm.DataModels;
using Ligg.RpaDesk.WinForm.Helpers;
using System;
using System.IO;
using System.Windows.Forms;
//using Ligg.RpaDesk.DataModels;

namespace Ligg.RpaDesk
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var startupDir = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
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
                if (formArgsArry.Length < 2) goto End;

                //by liggin2019 on 20200405
                //formArgsStr format
                //formTypeFlag=0/Szi/empty, SingZoneInterface，指单个Zone组成一个窗体
                //formTypeFlag^FormRelativeLocation^formTitle^initParams
                //formTypeFlag=1/Svi, SingViewInterface， 指单个View组成一个窗体
                //formTypeFlag^FormRelativeLocation^formTitle^initParams
                //formTypeFlag=2/Mvi, MultipleViewInterface，指多个View组成一个窗体
                //formTypeFlag^FormRelativeLocation|StartViewName^formTitle^initParams
                startArgs.FormTypeFlag = formArgsArry[0];
                var formType = FormType.Szi;
                startArgs.FormTitle = formArgsArry.Length > 2 ? formArgsArry[2] : "";

                if (startArgs.FormTypeFlag == "1" | startArgs.FormTypeFlag == "Svi") formType = FormType.Svi;
                else if (startArgs.FormTypeFlag == "2" | startArgs.FormTypeFlag == "Mvi") formType = FormType.Mvi;
                if (formType == FormType.Szi)
                {
                    startArgs.FormRelativeLocation = formArgsArry[1];
                }
                else if (formType == FormType.Svi)
                {
                    startArgs.FormRelativeLocation = formArgsArry[1];
                }
                else//--mvi
                {
                    var tmpArry = formArgsArry[1].GetLarrayArray(true, false);
                    startArgs.FormRelativeLocation = tmpArry[0];
                    startArgs.StartViewName = tmpArry.Length > 1 ? tmpArry[1] : "";
                    if (!startArgs.StartViewName.IsNullOrEmpty() & !StringExtension.AlphaNumeralAndHyphenExpression.IsMatch(startArgs.StartViewName)) throw new ArgumentException("mvi form StartViewName can only includes alpha, numeral, and hyphen! ");
                }

                if (DirectoryHelper.IsLegalAbsoluteDirectory(startArgs.FormRelativeLocation)) throw new ArgumentException("startArgs.FormRelativeLocation=" + startArgs.FormRelativeLocation + " can't be an absolute path! ");
                if (startArgs.FormRelativeLocation.StartsWith("~")) throw new ArgumentException("formLocation can't contain \"~\"! ");
                if (startArgs.FormRelativeLocation.StartsWith("\\")) startArgs.FormRelativeLocation = startArgs.FormRelativeLocation.Substring(1, startArgs.FormRelativeLocation.Length - 1);
                if (startArgs.FormRelativeLocation.EndsWith("\\")) startArgs.FormRelativeLocation = startArgs.FormRelativeLocation.Substring(0, startArgs.FormRelativeLocation.Length - 2);
                if (startArgs.FormRelativeLocation.StartsWith("\\") | startArgs.FormRelativeLocation.StartsWith("\\")) throw new ArgumentException("formLocation is incorrect! ");
                if (formType == FormType.Mvi)
                {
                    if (startArgs.FormRelativeLocation.GetQtyOfIncludedChar('\\') > 0) throw new ArgumentException("Mvi formLocation can only be single level!");
                    if (!StringExtension.AlphaNumeralAndHyphenExpression.IsMatch(startArgs.FormRelativeLocation)) throw new ArgumentException("Mvi formLocation can only includes alpha, numeral, hyphen! ");
                }
                else
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

                    if (!StringExtension.AlphaNumeralAndHyphenExpression.IsMatch(functionCode)) throw new ArgumentException("svi/szi formLocation last level folder can only includes alpha, numeral, hyphen! ");
                }

                startup.SetApplicationStartPolicy(startArgs.AppCode, formType, startArgs.FormRelativeLocation);


                //*subsequent actions by formArgStr

                //set formInitParamSet
                var formInitParamSet = new FormInitParamSet();
                formInitParamSet.FormType = formType;
                formInitParamSet.ArchitectureCode = CentralData.ArchitectureCode;
                formInitParamSet.ArchitectureName = CentralData.ArchitectureName;
                formInitParamSet.ArchitectureVersion = CentralData.ArchitectureVersion;
                formInitParamSet.OrganizationCode = CentralData.OrganizationCode;
                formInitParamSet.OrganizationShortName = CentralData.OrganizationShortName;
                formInitParamSet.OrganizationName = CentralData.OrganizationName;
                formInitParamSet.ApplicationCode = startArgs.AppCode;
                formInitParamSet.ApplicationVersion = appSetting.ApplicationVersion;

                formInitParamSet.FormRelativeLocation = startArgs.FormRelativeLocation;
                formInitParamSet.StartViewName = startArgs.StartViewName;


                formInitParamSet.FormTitle = startArgs.FormTitle ?? "";
                formInitParamSet.HelpdeskEmail = appSetting.HelpdeskEmail;
                formInitParamSet.SupportMultiLanguages = appSetting.SupportMultiLanguages;

                formInitParamSet.ApplicationLibDir = appSetting.ApplicationLibDir;
                formInitParamSet.ApplicationDataDir = appSetting.ApplicationDataDir;
                formInitParamSet.ApplicationOutputDir = appSetting.ApplicationOutputDir;

                formInitParamSet.HasCblpComponent = appSetting.HasCblpComponent;
                //ShowSoftwareCover
                if (appSetting.ShowSoftwareCoverAtStart)
                {
                    bool showSoftwareCover = startArgs.IsRedirectedFlag.ToLower() != "true";
                    if (showSoftwareCover)
                    {
                        startup.ShowSziForm(formInitParamSet, true);
                    }
                }

                var logon = appSetting.LogonAtStart;
                if (logon)
                {
                    //if (logonMode.ToLower() == "user")
                    {
                        //Explicit logon
                        if (logon)
                        {
                            if (logon) if (!startup.ShowSziForm(formInitParamSet, false)) goto End;
                        }
                    }
                }

                var form = new StartForm(formInitParamSet);
                Application.Run(form);
            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(TextRes.ApplicationStartError, TextRes.ApplicationStartError + ": " + ex.Message);
            }
        End:;
        }

    }
}
