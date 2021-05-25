using Ligg.EasyWinApp.Interface;
using Ligg.EasyWinApp.Resources;
using Ligg.EasyWinApp.WinForm.DataModel;
using Ligg.EasyWinApp.WinForm.DataModel.Enums;
using Ligg.EasyWinApp.WinForm.Helpers;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Utility.FileWrap;
using Ligg.EasyWinApp.Parser.DataModel;
using Ligg.EasyWinApp.Parser.DataModel.Enums;
using Ligg.EasyWinApp.Parser.Helpers;
using System;
using System.IO;
using System.Windows.Forms;

namespace Ligg.EasyWinApp
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var debugIniDir = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                var debugIniPath = debugIniDir + "\\Debug.ini";
                if (File.Exists(debugIniPath))
                {
                    string debugIniArgsStr = IniFileHelper.ReadIniString(debugIniPath, "setting", "args", "");
                    args = debugIniArgsStr.Split(' ');
                }
            }

            if (args.Length < 2) goto End;
            //# appArgsStr format
            //appCode ^formTitle^commonParams^startPassword^usrCode^userPassword^usrToken^invisibleFlag ^ isRedirectedFlag
            //#ov-
            //# appArgsStr = EncryptionHelper.SmDecrypt(args[0],EncryptionHelper.GlobalKey1,EncryptionHelper.GlobalKey2);
            var appArgStrArry = args[0].GetParamArray(true, false);
            //if (appArgStrArry.Length < 1) goto End;

            var appCode = appArgStrArry[0];
            var formTitle = "";
            var commonParams = "";
            var startPassword = "";
            var usrCode = "";
            var usrPassword = "";
            var usrToken = "";
            var invisibleFlag = "";
            var isRedirectedFlag = "";

            if (appArgStrArry.Length > 1) formTitle = appArgStrArry[1];
            if (appArgStrArry.Length > 2) commonParams = appArgStrArry[2];
            if (appArgStrArry.Length > 3) startPassword = appArgStrArry[3];
            if (appArgStrArry.Length > 4) usrCode = appArgStrArry[4];
            if (appArgStrArry.Length > 5) usrToken = appArgStrArry[5];
            if (appArgStrArry.Length > 6) usrPassword = appArgStrArry[6];
            if (appArgStrArry.Length > 7) invisibleFlag = appArgStrArry[7];
            if (appArgStrArry.Length > 8) isRedirectedFlag = appArgStrArry[8];

            var passedCultureName = "";
            if (args.Length > 2) passedCultureName = args[2];

            try
            {
                var startup = new Startup(appCode);
                var appStartParamSet = startup.ApplicationStartParamSet;
                if (appStartParamSet.SupportMultiCultures)
                {
                    startup.SetCultures(passedCultureName.IsNullOrEmpty() ? appStartParamSet.DefaultCulture : passedCultureName);
                }
                CentralData.InitApplicationSettings(appCode, appStartParamSet.SupportMultiCultures, CultureHelper.DefaultLanguageCode, CultureHelper.CurrentLanguageCode, commonParams,
                    appStartParamSet.ApplicationLibDir, appStartParamSet.ApplicationDataDir, appStartParamSet.PhythonExecPath);
                BootStrapper.Init(appStartParamSet);

                var uiType = UiType.Winform;
                if (uiType == UiType.Winform) //--winform starts
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    var winformArgsStr = args[1];

                    var funcArgsArry = winformArgsStr.GetParamArray(true, false);
                    if (funcArgsArry.Length < 2) goto End;

                    //##formArgsStr format
                    //###formTypeFlag=0, SviOfZone, SingViewInterfaceOfZone，指单个Zone组成一个窗体
                    //formTypeFlag^startSviZoneRelativeLocation^startZoneProcessParams

                    //###formTypeFlag=1, SviOfView, SingViewInterfaceOfView， 指单个View组成一个窗体
                    //formTypeFlag^startSviViewRelativeLocation

                    //###formTypeFlag=2, Mvi, MultipleViewInterface，指多个View组成一个窗体
                    //formTypeFlag^startFuncCode|startViewName


                    var formTypeFlag = funcArgsArry[0];
                    var formType = FormType.SviOfZone;
                    if (formTypeFlag == "1" | formTypeFlag.ToLower() == "SviView".ToLower()) formType = FormType.SviOfView;
                    else if (formTypeFlag == "2" | formTypeFlag.ToLower() == "Mvi".ToLower()) formType = FormType.Mvi;

                    var startSviZoneRelativeLocation = "";
                    var startZoneProcessParams = "";
                    var startSviViewRelativeLocation = "";
                    //--mvi
                    var startFuncCode = "";
                    var startViewName = "";
                    if (formType == FormType.SviOfZone)
                    {
                        startSviZoneRelativeLocation = funcArgsArry[1];
                        if (funcArgsArry.Length > 2) startZoneProcessParams = funcArgsArry[2];

                        if (FileHelper.IsAbsolutePath(startSviZoneRelativeLocation)) throw new ArgumentException("startSviZoneRelativeLocation can't be an absolute path! ");
                        if (startSviZoneRelativeLocation.StartsWith("~")) throw new ArgumentException("startSviZoneRelativeLocation can't contain \"~\"! ");

                    }
                    else if (formType == FormType.SviOfView)
                    {
                        startSviViewRelativeLocation = funcArgsArry[1];
                        if (FileHelper.IsAbsolutePath(startSviViewRelativeLocation)) throw new ArgumentException("startSviViewRelativeLocation can't be an absolute path! ");
                        if (startSviViewRelativeLocation.StartsWith("~")) throw new ArgumentException("startSviViewRelativeLocation can't contain \"~\"!  ");
                    }
                    else//--mvi
                    {
                        var tmpArry = funcArgsArry[1].GetSubParamArray(true, false);
                        startFuncCode = tmpArry[0];
                        startViewName = tmpArry.Length > 1 ? tmpArry[1] : "";

                        if (startFuncCode.IsNullOrEmpty()) throw new ArgumentException("startFuncCode can't be empty ");
                        if (!startFuncCode.IsAlphaNumeralAndHyphen()) throw new ArgumentException("startFuncCode Name can only includes alpha, numeral and hyphen! ");

                        if (!startViewName.IsNullOrEmpty() & !startViewName.IsAlphaNumeralAndHyphen()) throw new ArgumentException("startViewName can only includes alpha numeral and hyphen! ");

                    }

                    //#subsequent actions by winformArgStr
                    startup.SetApplicationStartPolicy(formType, formType == FormType.Mvi ? startFuncCode : funcArgsArry[1]);


                    if (!appStartParamSet.StyleSheetCode.IsNullOrEmpty())
                    {
                        //init Ligg.EasyWinApp.WinForm.StyleSheet
                    }

                    //##VerifyStartPassword
                    if (appStartParamSet.VerifyPasswordAtStart)
                    {
                        if (appStartParamSet.PasswordVerificationRule.IsNullOrEmpty()) throw new ArgumentException("PasswordVerificationRule in ApplicationSettings config file can not be empty! ");
                        if (!startPassword.IsNullOrEmpty())
                        {
                            if (!startup.VerifyStartPassword(false, appStartParamSet.PasswordVerificationRule, startPassword)) goto End;
                        }
                        else
                        {
                            if (!startup.VerifyStartPassword(true, appStartParamSet.PasswordVerificationRule, startPassword)) goto End;
                        }

                    }

                    //##set formInitParamSet
                    var formInitParamSet = new FormInitParamSet();

                    formInitParamSet.FormType = formType;
                    formInitParamSet.IsFormInvisible = invisibleFlag.GetJudgementFlag()=="true"?true : false;

                    formInitParamSet.ArchitectureCode = CentralData.ArchitectureCode;
                    formInitParamSet.ArchitectureName = CentralData.ArchitectureName;
                    formInitParamSet.ArchitectureVersion = CentralData.ArchitectureVersion;
                    formInitParamSet.OrganizationCode = CentralData.OrganizationCode;
                    formInitParamSet.OrganizationShortName = CentralData.OrganizationShortName;
                    formInitParamSet.OrganizationName = CentralData.OrganizationName;

                    formInitParamSet.ApplicationCode = appCode;
                    formInitParamSet.ApplicationVersion = appStartParamSet.ApplicationVersion;

                    formInitParamSet.FunctionCode = startFuncCode;
                    formInitParamSet.StartViewName = startViewName;
                    formInitParamSet.StartZoneProcessParams = startZoneProcessParams;
                    formInitParamSet.StartSviZoneRelativeLocation = startSviZoneRelativeLocation;
                    formInitParamSet.StartSviViewRelativeLocation = startSviViewRelativeLocation;


                    formInitParamSet.StartPassword = startup.StartPassword;
                    formInitParamSet.FormTitle = formTitle;
                    formInitParamSet.HelpdeskEmail = appStartParamSet.HelpdeskEmail;
                    formInitParamSet.SupportMultiCultures = appStartParamSet.SupportMultiCultures;
                    formInitParamSet.IsRedirected = isRedirectedFlag.ToLower() == "true" ? true : false;

                    formInitParamSet.ApplicationLibDir = appStartParamSet.ApplicationLibDir;
                    formInitParamSet.ApplicationDataDir = appStartParamSet.ApplicationDataDir;
                    formInitParamSet.ApplicationTempDir = appStartParamSet.ApplicationTempDir;

                    formInitParamSet.RunBootStrapperTasksAtStart = appStartParamSet.RunBootStrapperTasksAtStart;
                    formInitParamSet.HasCblpComponent = appStartParamSet.HasCblpComponent;

                    //##ShowSoftwareCover
                    if (appStartParamSet.ShowSoftwareCoverAtStart)
                    {
                        bool showSoftwareCover = isRedirectedFlag.ToLower() != "true";
                        if (showSoftwareCover)
                        {
                            var isOk = startup.ShowSoftwareCover(formInitParamSet);
                            if (!isOk) goto End;
                        }

                    }
                    else
                    {
                    }

                    var logon = false;
                    if (appStartParamSet.LogonAtStart) logon = true;

                    //##VerifyUserToken
                    if (logon & !usrCode.IsNullOrEmpty() & !usrToken.IsNullOrEmpty())
                    {
                        if (startup.VerifyUserToken(usrCode, usrToken))
                            logon = false;
                    }

                    //##VerifyUserPassword
                    else if (logon & !usrCode.IsNullOrEmpty() & !usrPassword.IsNullOrEmpty())
                    {
                        if (startup.VerifyUserPassword(usrCode, usrToken))
                            logon = false;
                    }

                    //##Logon
                    if (logon)
                    {
                        if (logon) if (!startup.Logon(formInitParamSet)) goto End;
                    }

                    var form = new StartForm(formInitParamSet);
                    Application.Run(form);


                }

            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(EasyWinAppRes.ApplicationStartError, EasyWinAppRes.ApplicationStartError + ": " + ex.Message);
            }
        End:;
        }



    }
}
