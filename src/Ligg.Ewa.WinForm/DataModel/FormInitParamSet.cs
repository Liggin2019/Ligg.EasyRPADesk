using Ligg.EasyWinApp.WinForm.DataModel.Enums;

namespace Ligg.EasyWinApp.WinForm.DataModel
{
    public class FormInitParamSet
    {
        public bool IsFormInvisible { get; set; }
        public FormType FormType { get; set; }
        public string ArchitectureCode { get; set; }
        public string ArchitectureName { get; set; }
        public string ArchitectureVersion { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationShortName { get; set; }
        public string OrganizationName { get; set; }

        public string ApplicationCode { get; set; }
        public string ApplicationVersion { get; set; }

        public string FunctionCode { get; set; }
        public string StartViewName { get; set; }

        public string StartSviViewRelativeLocation = "";
        public string StartSviZoneRelativeLocation = "";
        
        //public string StartCommonParams = "";
        public string StartZoneProcessParams = "";

        public string StartPassword { get; set; }
        public string HelpdeskEmail { get; set; }
        public bool SupportMultiCultures { get; set; }
        public string FormTitle { get; set; }

        public bool IsRedirected { get; set; }

        public string ApplicationDataDir { get; set; }
        public string ApplicationLibDir { get; set; }
        public string ApplicationTempDir { get; set; }

        public bool RunBootStrapperTasksAtStart { get; set; }
        public bool HasCblpComponent { get; set; }

    }

}
