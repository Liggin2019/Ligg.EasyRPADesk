using Ligg.RpaDesk.Interface;
using Ligg.RpaDesk.Parser.DataModels;
using Ligg.RpaDesk.Parser.Helpers;
using Ligg.RpaDesk.Resources;
using Ligg.RpaDesk.WinForm.DataModels;
using Ligg.RpaDesk.WinForm.Dialogs;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using  Ligg.Infrastructure.Utilities.DataParserUtil;


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
        internal bool ShowSziForm(FormInitParamSet formInitParamSet, bool showSoftwareCover)
        {
            var sziFormInitParamSet = new FormInitParamSet();
            sziFormInitParamSet.FormType = FormType.Szi;

            sziFormInitParamSet.ArchitectureCode = formInitParamSet.ArchitectureCode;
            sziFormInitParamSet.ArchitectureName = formInitParamSet.ArchitectureName;
            sziFormInitParamSet.ArchitectureVersion = formInitParamSet.ArchitectureVersion;
            sziFormInitParamSet.OrganizationCode = formInitParamSet.OrganizationCode;
            sziFormInitParamSet.OrganizationShortName = formInitParamSet.OrganizationShortName;
            sziFormInitParamSet.OrganizationName = formInitParamSet.OrganizationName;

            sziFormInitParamSet.ApplicationVersion = formInitParamSet.ApplicationVersion;
            sziFormInitParamSet.HelpdeskEmail = formInitParamSet.HelpdeskEmail;
            sziFormInitParamSet.SupportMultiLanguages = formInitParamSet.SupportMultiLanguages;
            sziFormInitParamSet.ApplicationCode = formInitParamSet.ApplicationCode;
            
            sziFormInitParamSet.FormTitle = formInitParamSet.FormTitle;
           

            sziFormInitParamSet.ApplicationDataDir = formInitParamSet.ApplicationDataDir;
            sziFormInitParamSet.ApplicationLibDir = formInitParamSet.ApplicationLibDir;
            sziFormInitParamSet.ApplicationOutputDir = formInitParamSet.ApplicationOutputDir;

            var form = new StartForm(sziFormInitParamSet);
            if (showSoftwareCover)
            {
                var loc = "_Start\\SoftwareCover";
                sziFormInitParamSet.FormRelativeLocation = loc;
                form.ShowDialog();
                //Application.Run(form);
                form.BoolOutput = true;
                if (!form.BoolOutput) return false;
            }
            else
            {
                var loc = "_Start\\Logon"; 
                sziFormInitParamSet.FormRelativeLocation = loc;
                Application.Run(form);
                if (!form.BoolOutput) return false;
            }

            return true;
        }



    }
}