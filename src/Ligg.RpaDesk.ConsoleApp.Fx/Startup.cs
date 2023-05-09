using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.RpaDesk.Parser.DataModels;
using Ligg.RpaDesk.Interface;
using Ligg.RpaDesk.Parser.Helpers;
using Ligg.RpaDesk.Resources;
using System;
using System.Collections.Generic;
using System.IO;


namespace Ligg.RpaDesk
{
    internal class Startup
    {
        internal ApplicationSetting ApplicationSetting;
        internal Startup(string appCode)
        {
            try
            {
                CentralData.InitCommonSetting();
                ApplicationSetting = StartupHelper.GetApplicationSetting<ApplicationSetting>(CentralData.ArchitectureCode, appCode);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".Startup Error: " + ex.Message);
            }
        }


        //*set
        internal void SetApplicationStartPolicy(string appCode, FormType formType, string formRelativePath)
        {
            var frmStartPolicy = StartupHelper.GetFormStartPolicy<FormStartPolicy>(appCode, formType, formRelativePath);
            ApplicationSetting.LogonAtStart = frmStartPolicy == null ? ApplicationSetting.LogonAtStart : frmStartPolicy.LogonAtStart;
            ApplicationSetting.ShowSoftwareCoverAtStart = frmStartPolicy == null ? ApplicationSetting.ShowSoftwareCoverAtStart : frmStartPolicy.ShowSoftwareCoverAtStart;
        }

        //*act
        internal bool RunScenario(FormInitParamSet formInitParamSet, bool showSoftwareCover)
        {
            var snrFormInitParamSet = new FormInitParamSet();

            snrFormInitParamSet.ArchitectureCode = formInitParamSet.ArchitectureCode;
            snrFormInitParamSet.ArchitectureName = formInitParamSet.ArchitectureName;
            snrFormInitParamSet.ArchitectureVersion = formInitParamSet.ArchitectureVersion;
            snrFormInitParamSet.OrganizationCode = formInitParamSet.OrganizationCode;
            snrFormInitParamSet.OrganizationShortName = formInitParamSet.OrganizationShortName;
            snrFormInitParamSet.OrganizationName = formInitParamSet.OrganizationName;

            snrFormInitParamSet.ApplicationVersion = formInitParamSet.ApplicationVersion;
            snrFormInitParamSet.HelpdeskEmail = formInitParamSet.HelpdeskEmail;
            snrFormInitParamSet.SupportMultiLanguages = formInitParamSet.SupportMultiLanguages;
            snrFormInitParamSet.ApplicationCode = formInitParamSet.ApplicationCode;

            snrFormInitParamSet.FormTitle = formInitParamSet.FormTitle;
            

            snrFormInitParamSet.ApplicationDataDir = formInitParamSet.ApplicationDataDir;
            snrFormInitParamSet.ApplicationLibDir = formInitParamSet.ApplicationLibDir;
            snrFormInitParamSet.ApplicationOutputDir = formInitParamSet.ApplicationOutputDir;

            var loc = showSoftwareCover ? "_Start\\SoftwareCover" : "_Start\\Logon";
            snrFormInitParamSet.FormRelativeLocation = loc;
            var form = new StartForm(snrFormInitParamSet);
            if (!showSoftwareCover)
            {
                if (!form.BoolOutput) return false;
            }

            return true;
        }


    }
}