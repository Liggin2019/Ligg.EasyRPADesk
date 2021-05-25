
namespace Ligg.EasyWinApp.WinCnsl.DataModel
{
    public class FormInitParamSet
    {
        public string InvisibleFlag { get; set; }
        //public FormType FormType { get; set; }
        public string ArchitectureCode { get; set; }
        public string ArchitectureName { get; set; }
        public string ArchitectureVersion { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationShortName { get; set; }
        public string OrganizationName { get; set; }

        public string ApplicationCode { get; set; }
        public string ApplicationVersion { get; set; }

        public string StartScenarioRelativeLocation = "";
        public string StartScenarioProcessParams = "";
        public string ScenarioCode = "";

        //public string StartActions { get; set; }
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
