using Ligg.EasyWinApp.Interface;
using Ligg.EasyWinApp.Resources;
using Ligg.EasyWinApp.WinForm.DataModel;
using Ligg.EasyWinApp.WinForm.DataModel.Enums;
using Ligg.EasyWinApp.WinForm.Dialogs;
using Ligg.EasyWinApp.WinForm.Helpers;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Utility.FileWrap;

using Ligg.EasyWinApp.Parser.DataModel;
using Ligg.EasyWinApp.Parser.DataModel.Enums;
using Ligg.EasyWinApp.Parser;
using Ligg.EasyWinApp.Parser.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;


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
            try
            {
                CentralData.InitCommonSettings();
                //*ov-
                //EncryptionHelper.Key1 = GlobalConfiguration.GlobalKey2;
                //EncryptionHelper.Key2 = GlobalConfiguration.GlobalKey2;

                var executableDir = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                _startUpDir = Directory.GetParent(executableDir).ToString();
                _startUpDir = Directory.GetParent(_startUpDir).ToString();
                _startUpDir = Directory.GetParent(_startUpDir).ToString();
                Directory.SetCurrentDirectory(_startUpDir);
                _cfgDir = _startUpDir + "\\Conf";
                var cfgFile = _cfgDir + "\\GlobalSetting";
                if (!ConfigFileHelper.IsFileExisting(cfgFile)) throw new ArgumentException(" Must have a \"GlobalSetting\" config file in " + _cfgDir);
                var cfgFileMgr = new ConfigFileManager(cfgFile);
                var globalSetting = cfgFileMgr.ConvertToGeneric<GlobalSetting>();

                _appCfgDir = _cfgDir + "\\Apps\\" + appCode;
                cfgFile = _appCfgDir + "\\ApplicationSetting";
                if (!ConfigFileHelper.IsFileExisting(cfgFile)) throw new ArgumentException(" Must have a \"ApplicationSetting\" config file in " + _appCfgDir);
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
                    appTempDir = PathHelper.GetLastDriveName() + "\\tmp\\" + CentralData.ArchitectureCode + "\\" + appCode;
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

                //# following exchange data with globalsetting
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
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".Startup Error: " + ex.Message);
            }
        }

        internal void SetCultures(string cultureName)
        {
            try
            {
                var cfgFile = _cfgDir + "\\Cultures\\Cultures";
                if (!ConfigFileHelper.IsFileExisting(cfgFile)) return;
                var cfgFileMgr = new ConfigFileManager(cfgFile);
                var cultures = cfgFileMgr.ConvertToGeneric<List<Culture>>();
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
                //AnnexHelper.DefaultLanguageCode = CultureHelper.DefaultLanguageCode;

            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".SetCultures Error: " + ex.Message);
            }
        }

        //# set
        internal void SetApplicationStartPolicy(FormType formType, string funcCodeOrRelativePath)
        {

            var formCFgDir = "";

            if (formType == FormType.Mvi)
            {
                formCFgDir = _appCfgDir + "\\UIs\\WinForm\\Functions\\" + funcCodeOrRelativePath;
            }
            else if (formType == FormType.SviOfView)
            {
                formCFgDir = _appCfgDir + "\\UIs\\WinForm\\Views\\" + funcCodeOrRelativePath;
            }
            else
            {
                formCFgDir = _appCfgDir + "\\UIs\\WinForm\\Zones\\" + funcCodeOrRelativePath;
            }
            if (!DirectoryHelper.IsDirectoryExisting(formCFgDir)) throw new ArgumentException(String.Format("Forn config Folder: {0} doesn't exist! ", formCFgDir));

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




        //#act
        internal bool VerifyStartPassword(bool verifyByInput, string passwordVerificationRule, string password)
        {
            try
            {
                if (!passwordVerificationRule.IsNullOrEmpty())
                {
                    if (passwordVerificationRule.Contains("；")) passwordVerificationRule = passwordVerificationRule.Replace("；", ";");
                    if (passwordVerificationRule.Contains("，")) passwordVerificationRule = passwordVerificationRule.Replace("，", ",");
                    var verifyType = passwordVerificationRule.SplitByTwoDifferentStrings("Type:", ";", true)[0];
                    var verifyParams = passwordVerificationRule.SplitByTwoDifferentStrings("Params:", ";", true)[0];

                    if (!EnumHelper.IsNameValid<PasswordEncryptionType>(verifyType)) throw new ArgumentException("passwordVerificationRule is not correct!");

                    if (verifyByInput)
                    {
                        StartPassword = verifyParams;
                        var dlg = new TextInputDialog();
                        {
                            dlg.Text = EasyWinAppRes.PlsInputPassword;
                            dlg.VerificationRule = verifyType;
                            dlg.VerificationParams = verifyParams;
                            dlg.ShowDialog();
                            return dlg.IsOk;

                        }
                    }
                    else
                    {
                        if (GetHelper.VerifyPassword(password, verifyType, verifyParams))
                        {
                            StartPassword = verifyParams;
                            return true;
                        }
                        else return false;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".VerifyStartPassword Error: " + ex.Message);
            }
            return true;

        }

        internal bool VerifyUserToken(string userCode, string userToken)
        {
            return BootStrapper.Adapter.VerifyUserToken(userCode, userToken);
        }

        internal bool VerifyUserPassword(string userCode, string userPassword)
        {
            return BootStrapper.Adapter.Logon(userCode, userPassword);
        }

        //##SviZoneForm act
        internal bool ShowSoftwareCover(FormInitParamSet formInitParamSet)
        {

            var sviZoneFormInitParamSet = new FormInitParamSet();
            sviZoneFormInitParamSet.FormType = FormType.SviOfZone;

            sviZoneFormInitParamSet.ArchitectureCode = formInitParamSet.ArchitectureCode;
            sviZoneFormInitParamSet.ArchitectureName = formInitParamSet.ArchitectureName;
            sviZoneFormInitParamSet.ArchitectureVersion = formInitParamSet.ArchitectureVersion;
            sviZoneFormInitParamSet.OrganizationCode = formInitParamSet.OrganizationCode;
            sviZoneFormInitParamSet.OrganizationShortName = formInitParamSet.OrganizationShortName;
            sviZoneFormInitParamSet.OrganizationName = formInitParamSet.OrganizationName;

            //sviZoneFormInitParamSet.IsFormInvisible = true;
            sviZoneFormInitParamSet.ApplicationCode = formInitParamSet.ApplicationCode;
            sviZoneFormInitParamSet.ApplicationVersion = formInitParamSet.ApplicationVersion;

            //sviZoneFormInitParamSet.FunctionCode ="ShowSoftwareCover";
            sviZoneFormInitParamSet.StartSviZoneRelativeLocation = ApplicationStartParamSet.SoftwareCoverLocation;
            sviZoneFormInitParamSet.StartZoneProcessParams = formInitParamSet.FunctionCode;
            sviZoneFormInitParamSet.HelpdeskEmail = formInitParamSet.HelpdeskEmail;
            sviZoneFormInitParamSet.SupportMultiCultures = formInitParamSet.SupportMultiCultures;
            sviZoneFormInitParamSet.IsRedirected = false;

            var form = new StartForm(sviZoneFormInitParamSet);
            Application.Run(form);

            if (!form.BoolValue) return false;
            return true;


        }

        internal bool Logon(FormInitParamSet formInitParamSet)
        {

            var sviZoneFormInitParamSet = new FormInitParamSet();
            sviZoneFormInitParamSet.FormType = FormType.SviOfZone;

            sviZoneFormInitParamSet.ArchitectureCode = formInitParamSet.ArchitectureCode;
            sviZoneFormInitParamSet.ArchitectureName = formInitParamSet.ArchitectureName;
            sviZoneFormInitParamSet.ArchitectureVersion = formInitParamSet.ArchitectureVersion;
            sviZoneFormInitParamSet.OrganizationCode = formInitParamSet.OrganizationCode;
            sviZoneFormInitParamSet.OrganizationShortName = formInitParamSet.OrganizationShortName;
            sviZoneFormInitParamSet.OrganizationName = formInitParamSet.OrganizationName;

            //sviZoneFormInitParamSet.IsFormInvisible = true;
            sviZoneFormInitParamSet.ApplicationCode = formInitParamSet.ApplicationCode;
            sviZoneFormInitParamSet.ApplicationVersion = formInitParamSet.ApplicationVersion;

            //sviZoneFormInitParamSet.FunctionCode ="ShowSoftwareCover";
            sviZoneFormInitParamSet.StartSviZoneRelativeLocation = ApplicationStartParamSet.LogonLocation;
            sviZoneFormInitParamSet.StartZoneProcessParams = "";
            sviZoneFormInitParamSet.HelpdeskEmail = formInitParamSet.HelpdeskEmail;
            sviZoneFormInitParamSet.SupportMultiCultures = formInitParamSet.SupportMultiCultures;

            sviZoneFormInitParamSet.IsRedirected = false;
            var form = new StartForm(sviZoneFormInitParamSet);
            Application.Run(form);
            if (!form.BoolValue) return false;
            return true;
        }


    }
}