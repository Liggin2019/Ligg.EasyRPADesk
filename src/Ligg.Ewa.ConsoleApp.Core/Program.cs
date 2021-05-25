using Ligg.EasyWinApp.Interface;
using Ligg.EasyWinApp.Resources;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Utility.FileWrap;
using Ligg.EasyWinApp.WinCnsl.DataModel;
using Ligg.EasyWinApp.Parser.Helpers;
using System;
using System.IO;
using System.Diagnostics;

namespace Ligg.EasyWinApp
{
    static class Program
    {

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var debugIniDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                var debugIniPath = debugIniDir + "\\Debug.ini";
                if (File.Exists(debugIniPath))
                {
                    string debugIniArgsStr = IniFileHelper.ReadIniString(debugIniPath, "setting", "args", "");
                    args = debugIniArgsStr.Split(' ');
                }
            }

            if (args.Length < 2) goto End;
            //# appArgStr format
            //appCode ^formTitle^commonParams^startPassword^usrCode^userPassword^usrToken^invisibleFlag ^ isRedirectedFlag

            //#ov-
            //# appArgStr = EncryptionHelper.SmDecrypt(args[0],EncryptionHelper.GlobalKey1,EncryptionHelper.GlobalKey2);
            var appArgStrArry = args[0].GetParamArray(true, false);

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
                ConsoleHelper.DisbleQuickEditMode();
                var startup = new Startup(appCode);
                var appStartParamSet = startup.ApplicationStartParamSet;
                if (appStartParamSet.SupportMultiCultures)
                {
                    startup.SetCultures(passedCultureName.IsNullOrEmpty() ? appStartParamSet.DefaultCulture : passedCultureName);
                }
                CentralData.InitApplicationSettings(appCode, appStartParamSet.SupportMultiCultures, CultureHelper.DefaultLanguageCode, CultureHelper.CurrentLanguageCode,commonParams
                    , appStartParamSet.ApplicationLibDir, appStartParamSet.ApplicationDataDir, appStartParamSet.PhythonExecPath);
                BootStrapper.Init(appStartParamSet);
                appStartParamSet.Invisible = invisibleFlag;

                var winCnslArgStr = args[1];

                var winCnslArgStrArry = winCnslArgStr.GetParamArray(true, false);

                if (winCnslArgStrArry.Length < 1) goto End;

                //## funcArgs format
                //startScenarioRelativeLocation^startScenarioProcessParams
                var startScenarioRelativeLocation = winCnslArgStrArry[0];
                if (FileHelper.IsAbsolutePath(startScenarioRelativeLocation)) throw new ArgumentException("startScenarioRelativeLocation can't be an absolute path! ");
                if (startScenarioRelativeLocation.StartsWith("~")) throw new ArgumentException("startScenarioRelativeLocation can't contain \"~\"! ");

                var startScenarioProcessParams = "";
                if (winCnslArgStrArry.Length > 1) startScenarioProcessParams = winCnslArgStrArry[1];

                startup.SetApplicationStartPolicy(startScenarioRelativeLocation);

                //##VerifyStartPassword
                if (appStartParamSet.VerifyPasswordAtStart)
                {
                    if (appStartParamSet.PasswordVerificationRule.IsNullOrEmpty()) throw new ArgumentException("PasswordVerificationRule in ApplicationSetting config file can not be empty! ");
                    if (!startPassword.IsNullOrEmpty())
                    {
                        if (!startup.VerifyStartPassword(false, appStartParamSet.PasswordVerificationRule, startPassword)) goto End;
                    }
                    else
                    {
                        if (!startup.VerifyStartPassword(true, appStartParamSet.PasswordVerificationRule, startPassword)) goto End;
                    }
                    Console.Clear();
                }

                //##set formInitParamSet
                var formInitParamSet = new FormInitParamSet();

                formInitParamSet.ArchitectureCode = CentralData.ArchitectureCode;
                formInitParamSet.ArchitectureName = CentralData.ArchitectureName;
                formInitParamSet.ArchitectureVersion = CentralData.ArchitectureVersion;
                formInitParamSet.OrganizationCode = CentralData.OrganizationCode;
                formInitParamSet.OrganizationShortName = CentralData.OrganizationShortName;
                formInitParamSet.OrganizationName = CentralData.OrganizationName;

                formInitParamSet.InvisibleFlag = appStartParamSet.Invisible;
                formInitParamSet.ApplicationCode = appCode;
                formInitParamSet.ApplicationVersion = appStartParamSet.ApplicationVersion??"";

                formInitParamSet.StartScenarioRelativeLocation = startScenarioRelativeLocation;
                //formInitParamSet.StartCommonParams = startCommonParams; //? required? it can be get from CentralData
                formInitParamSet.StartScenarioProcessParams = startScenarioProcessParams;

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
                //var cblpDllPath = appStartParamSet.CblpDllPath;
                //if (!cblpDllPath.IsNullOrEmpty()) ApplicationAdapter.Init1(debug, cblpDllPath, appStartParamSet.CblpAdapterClassFullName);

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

                var form = new FunctionForm(formInitParamSet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(EasyWinAppRes.PressAnyKeyToExit+" !"); 
                Console.ReadKey();
                ConsoleHelper.CloseConsole();
            }
        End:;
        }



    }
}
