using Ligg.EasyWinApp.Interface;
using Ligg.EasyWinApp.Resources;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Utility.FileWrap;
using Ligg.EasyWinApp.WinCnsl.DataModel;
using Ligg.EasyWinApp.Parser.DataModel;
using Ligg.EasyWinApp.Parser.DataModel.Enums;
using Ligg.EasyWinApp.Parser.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;


namespace Ligg.EasyWinApp
{
    internal class Startup
    {
        internal string DefaultCultureName = "";
        private string _startUpDir = "";
        private string _cfgDir = "";
        private string _appCfgDir = "";

        internal string StartPassword = "";
        internal ApplicationSetting ApplicationStartParamSet;

        internal Startup(string appCode)
        {
            CentralData.InitCommonSettings();
            //*ov-
            //EncryptionHelper.Key1 = GlobalConfiguration.GlobalKey2;
            //EncryptionHelper.Key2 = GlobalConfiguration.GlobalKey2;
            var executableDir = Path.GetDirectoryName(PathHelper.GetMainModuleFileName());
            _startUpDir = Directory.GetParent(executableDir).ToString();
            _startUpDir = Directory.GetParent(_startUpDir).ToString();
            _startUpDir = Directory.GetParent(_startUpDir).ToString();
            Directory.SetCurrentDirectory(_startUpDir);
            _cfgDir = _startUpDir + "\\Conf";
            var cfgFile = _cfgDir + "\\GlobalSetting";
            if (!ConfigFileHelper.IsFileExisting(cfgFile)) throw new ArgumentException("Must have a \"GlobalSetting\" config file in " + _cfgDir);
            var cfgFileMgr = new ConfigFileManager(cfgFile);
            var globalSetting = cfgFileMgr.ConvertToGeneric<GlobalSetting>();

            _appCfgDir = _cfgDir + "\\Apps\\" + appCode;
            cfgFile = _appCfgDir + "\\ApplicationSetting";
            if (!ConfigFileHelper.IsFileExisting(cfgFile)) throw new ArgumentException("Must have a \"ApplicationSetting\" config file in " + _appCfgDir);
            cfgFileMgr = new ConfigFileManager(cfgFile);
            ApplicationStartParamSet = cfgFileMgr.ConvertToGeneric<ApplicationSetting>();

            var appDataDir = "";
            if (ApplicationStartParamSet.ApplicationDataDir.IsNullOrEmpty())
            {
                appDataDir = _startUpDir + "\\Data\\Apps\\" + appCode;
            }
            else
            {
                appDataDir = FileHelper.GetFilePath(ApplicationStartParamSet.ApplicationDataDir, _startUpDir + "\\Data" + appCode);
            }
            ApplicationStartParamSet.ApplicationDataDir = appDataDir;

            var appLibDir = "";
            if (ApplicationStartParamSet.ApplicationLibDir.IsNullOrEmpty())
            {
                appLibDir = _startUpDir + "\\Lib\\Apps\\" + appCode;
            }
            else
            {
                appLibDir = FileHelper.GetFilePath(ApplicationStartParamSet.ApplicationLibDir, _startUpDir + "\\Lib" + appCode);
            }
            ApplicationStartParamSet.ApplicationLibDir = appLibDir;

            var appTempDir = "";
            if (ApplicationStartParamSet.ApplicationTempDir.IsNullOrEmpty())
            {
                var drivers = DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Fixed).ToList();
                var n = drivers.Count - 1;
                
                appTempDir = PathHelper.GetLastDriveName()+ "\\tmp\\" + CentralData.ArchitectureCode + "\\" + appCode;
            }
            else
            {
                appTempDir = FileHelper.GetFilePath(ApplicationStartParamSet.ApplicationTempDir, _startUpDir + "\\Data\\" + appCode + "\\Tmp");
            }
            ApplicationStartParamSet.ApplicationTempDir = appTempDir;



            var cblpDllPath = "";
            if (ApplicationStartParamSet.CblpDllPath.IsNullOrEmpty())
            {
                cblpDllPath = _startUpDir + "\\Program\\Apps\\" + appCode + "\\Cblp." + appCode + ".dll";
            }
            else if (ApplicationStartParamSet.CblpDllPath.ToLower() == "none" | ApplicationStartParamSet.CblpDllPath.ToLower() == "no")
            {
                cblpDllPath = "";
            }
            else
            {
                cblpDllPath = FileHelper.GetFilePath(ApplicationStartParamSet.CblpDllPath, _startUpDir + "\\Program\\Apps\\" + appCode);
            }
            ApplicationStartParamSet.CblpDllPath = cblpDllPath;

            var cblpAdapterFullClassName = "";
            if (ApplicationStartParamSet.CblpAdapterClassFullName.IsNullOrEmpty())
            {
                cblpAdapterFullClassName = "Ligg.EasyWinApp.Cblp." + appCode + "Adapter";
            }
            ApplicationStartParamSet.CblpAdapterClassFullName = cblpAdapterFullClassName;

            //#followings exchange data with globalsetting
            var bootStrapperTasksDllPath = "";
            if (globalSetting.BootStrapperTasksDllPath.IsNullOrEmpty())
            {
                bootStrapperTasksDllPath = _startUpDir + "\\Program\\BootStrapperTasks\\" + "BootStrapperTasks" + ".dll";
            }
            else
            {
                bootStrapperTasksDllPath = FileHelper.GetFilePath(globalSetting.BootStrapperTasksDllPath, _startUpDir + "\\Program\\BootStrapperTasks");
            }
            ApplicationStartParamSet.BootStrapperTasksDllPath = bootStrapperTasksDllPath;

            if (globalSetting.BootStrapperTaskNameSpace.IsNullOrEmpty())
            {
                ApplicationStartParamSet.BootStrapperTaskNameSpace = "Ligg.EasyWinApp.BootStrapperTasks";
            }

            if (!globalSetting.PhythonExePath.IsNullOrEmpty())
            {
                var defLoc = Directory.GetParent(appLibDir).ToString();
                defLoc = Directory.GetParent(defLoc).ToString();
                var phythonExePath = FileHelper.GetFilePath(globalSetting.PhythonExePath, defLoc);
                ApplicationStartParamSet.PhythonExecPath = phythonExePath;
                SysProcessHelper.PythonExecPath = phythonExePath;
            }
            if (ApplicationStartParamSet.DefaultCulture.IsNullOrEmpty()) ApplicationStartParamSet.DefaultCulture = globalSetting.DefaultCulture;
            if (ApplicationStartParamSet.HelpdeskEmail.IsNullOrEmpty()) ApplicationStartParamSet.HelpdeskEmail = globalSetting.HelpdeskEmail;


        }

        internal void SetCultures(string cultureName)
        {

            var cfgFile = _cfgDir + "\\Cultures\\Cultures";
            if (!ConfigFileHelper.IsFileExisting(cfgFile)) return;
            var cultures = new List<Culture>();
            try
            {
                var cfgFileMgr = new ConfigFileManager(cfgFile);
                cultures = cfgFileMgr.ConvertToGeneric<List<Culture>>();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".SetCultures Error: " + ex.Message);
            }
            if (cultures != null)
            {
                CultureHelper.Cultures = cultures;
                var defCulture = cultures.Find(x => x.IsDefault);
                if (defCulture != null)
                {
                    this.DefaultCultureName = defCulture.Name;
                }
                else
                {
                    this.DefaultCultureName = cultures.First().Name;
                }

            }
            else
            {
                throw new ArgumentException("Can't get valid Cultures!");
            }

            var curCultureName = DefaultCultureName;
            if (!cultureName.IsNullOrEmpty() && CultureHelper.IsCultureNameValid(cultureName))
            {
                curCultureName = cultureName;
            }
            CultureHelper.SetCurrentCulture(curCultureName);


        }

        //#set

        internal void SetApplicationStartPolicy(string scenarioRelativePath)
        {

            var formCFgDir = _appCfgDir + "\\Uis\\Console\\Scenarios\\" + scenarioRelativePath;
            if (!DirectoryHelper.IsDirectoryExisting(formCFgDir)) throw new ArgumentException(String.Format("Scenario Folder: {0} doesn't exist! ", formCFgDir));

            var startPolicy = new StartPolicy();
            var cfgFile = formCFgDir + "\\StartPolicy";

            if (!ConfigFileHelper.IsFileExisting(cfgFile)) return;
            try
            {
                var confFileMgr = new ConfigFileManager(cfgFile);
                startPolicy = confFileMgr.ConvertToGeneric<StartPolicy>();

                ApplicationStartParamSet.VerifyPasswordAtStart = startPolicy.VerifyPasswordAtStartFlag.ToLower() == "false" ? false
                    : (startPolicy.VerifyPasswordAtStartFlag.ToLower() == "true" ? true : ApplicationStartParamSet.VerifyPasswordAtStart);

                ApplicationStartParamSet.ShowSoftwareCoverAtStart = startPolicy.ShowSoftwareCoverAtStartFlag.ToLower() == "false" ? false
                    : (startPolicy.ShowSoftwareCoverAtStartFlag.ToLower() == "true" ? true : ApplicationStartParamSet.ShowSoftwareCoverAtStart);

                ApplicationStartParamSet.LogonAtStart = startPolicy.LogonAtStartFlag.ToLower() == "false" ? false
                    : (startPolicy.LogonAtStartFlag.ToLower() == "true" ? true : ApplicationStartParamSet.LogonAtStart);

                ApplicationStartParamSet.RunBootStrapperTasksAtStart = startPolicy.RunBootStrapperTasksAtStartFlag.ToLower() == "false" ? false
                    : (startPolicy.RunBootStrapperTasksAtStartFlag.ToLower() == "true" ? true : ApplicationStartParamSet.RunBootStrapperTasksAtStart);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".SetApplicationStartParamSet Error: " + ex.Message);
            }

        }



        //# act
        internal bool VerifyStartPassword(bool verifyByInput, string passwordVerificationRule, string password)
        {

            if (!passwordVerificationRule.IsNullOrEmpty())
            {
                if (passwordVerificationRule.Contains("；")) passwordVerificationRule = passwordVerificationRule.Replace("；", ";");
                if (passwordVerificationRule.Contains("，")) passwordVerificationRule = passwordVerificationRule.Replace("，", ",");

                var verifyTypeStr = passwordVerificationRule.SplitByTwoDifferentStrings("Type:", ";", true)[0];
                var verifyParams = passwordVerificationRule.SplitByTwoDifferentStrings("Params:", ";", true)[0];

                if (!EnumHelper.IsNameValid<PasswordEncryptionType>(verifyTypeStr)) throw new ArgumentException("passwordVerificationRule is not correct!");
                if (verifyByInput)
                {

                    var i = 6;
                    do
                    {
                        Console.Write(EasyWinAppRes.PlsInputPassword + ": ");
                        var pass = Console.ReadLine();
                        if (GetHelper.VerifyPassword(pass, verifyTypeStr, verifyParams))
                        {
                            StartPassword = verifyParams;
                            return true;
                        }
                        Console.WriteLine(string.Format(EasyWinAppRes.PasswordNotCorrectTryAgain, i - 1));
                        i--;
                    }
                    while (i > 0);
                }
                else
                {
                    if (GetHelper.VerifyPassword(password, verifyTypeStr, verifyParams))
                    {
                        StartPassword = verifyParams;
                        return true;
                    }
                }

            }
            return false;
        }

        internal bool VerifyUserToken(string userCode, string userToken)
        {
            return BootStrapper.Adapter.VerifyUserToken(userCode, userToken);
        }

        internal bool VerifyUserPassword(string userCode, string userPassword)
        {
            return BootStrapper.Adapter.Logon(userCode, userPassword);
        }


        internal bool ShowSoftwareCover(FormInitParamSet formInitParamSet)
        {
            var formInitParamSet1 = new FormInitParamSet();
            formInitParamSet1.ArchitectureCode = formInitParamSet.ArchitectureCode;
            formInitParamSet1.ArchitectureName = formInitParamSet.ArchitectureName;
            formInitParamSet1.ArchitectureVersion = formInitParamSet.ArchitectureVersion;
            formInitParamSet1.OrganizationCode = formInitParamSet.OrganizationCode;
            formInitParamSet1.OrganizationShortName = formInitParamSet.OrganizationShortName;
            formInitParamSet1.OrganizationName = formInitParamSet.OrganizationName;
            formInitParamSet1.InvisibleFlag = "false";
            formInitParamSet1.ApplicationCode = formInitParamSet.ApplicationCode;
            formInitParamSet1.ApplicationVersion = formInitParamSet.ApplicationVersion;

            formInitParamSet1.StartScenarioRelativeLocation = ApplicationStartParamSet.SoftwareCoverLocation;
            formInitParamSet1.StartScenarioProcessParams = "";

            formInitParamSet1.StartPassword = "";
            formInitParamSet1.FormTitle = "";
            formInitParamSet1.HelpdeskEmail = formInitParamSet.HelpdeskEmail;
            formInitParamSet1.SupportMultiCultures = formInitParamSet.SupportMultiCultures;
            formInitParamSet1.IsRedirected = false;

            formInitParamSet1.ApplicationLibDir = formInitParamSet.ApplicationLibDir;
            formInitParamSet1.ApplicationDataDir = formInitParamSet.ApplicationDataDir;
            formInitParamSet1.ApplicationTempDir = formInitParamSet.ApplicationTempDir;
            formInitParamSet1.RunBootStrapperTasksAtStart = false;


            var form = new FunctionForm(formInitParamSet1);

            return true;
        }

        internal bool Logon(FormInitParamSet formInitParamSet)
        {

            var formInitParamSet1 = new FormInitParamSet();
            formInitParamSet1.ArchitectureCode = formInitParamSet.ArchitectureCode;
            formInitParamSet1.ArchitectureName = formInitParamSet.ArchitectureName;
            formInitParamSet1.ArchitectureVersion = formInitParamSet.ArchitectureVersion;
            formInitParamSet1.OrganizationCode = formInitParamSet.OrganizationCode;
            formInitParamSet1.OrganizationShortName = formInitParamSet.OrganizationShortName;
            formInitParamSet1.OrganizationName = formInitParamSet.OrganizationName;
            formInitParamSet1.InvisibleFlag = "false";
            formInitParamSet1.ApplicationCode = formInitParamSet.ApplicationCode;
            formInitParamSet1.ApplicationVersion = formInitParamSet.ApplicationVersion;

            formInitParamSet1.StartScenarioRelativeLocation = ApplicationStartParamSet.LogonLocation;
            formInitParamSet1.StartScenarioProcessParams = "";

            formInitParamSet1.StartPassword = "";
            formInitParamSet1.FormTitle = "";
            formInitParamSet1.HelpdeskEmail = formInitParamSet.HelpdeskEmail;
            formInitParamSet1.SupportMultiCultures = formInitParamSet.SupportMultiCultures;
            formInitParamSet1.IsRedirected = false;

            formInitParamSet1.ApplicationLibDir = formInitParamSet.ApplicationLibDir;
            formInitParamSet1.ApplicationDataDir = formInitParamSet.ApplicationDataDir;
            formInitParamSet1.ApplicationTempDir = formInitParamSet.ApplicationTempDir;
            formInitParamSet1.RunBootStrapperTasksAtStart = false;


            var form = new FunctionForm(formInitParamSet1);
            if (!form.BoolValue) return false;
            return true;
        }


    }
}