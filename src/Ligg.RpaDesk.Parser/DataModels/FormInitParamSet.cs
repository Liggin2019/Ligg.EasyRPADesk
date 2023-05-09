
namespace Ligg.RpaDesk.Parser.DataModels
{
    public class FormInitParamSet
    {
        public FormType FormType { get; set; }
        public string ArchitectureCode { get; set; }
        public string ArchitectureName { get; set; }
        public string ArchitectureVersion { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationShortName { get; set; }
        public string OrganizationName { get; set; }

        public bool SupportMultiLanguages { get; set; }
        public string HelpdeskEmail { get; set; }
        public string ApplicationVersion { get; set; }

        public string ApplicationCode { get; set; }
        public string FormRelativeLocation = "";
        public string FormTitle { get; set; }

        public string ApplicationDataDir { get; set; }
        public string ApplicationLibDir { get; set; }
        public string ApplicationOutputDir { get; set; }

        public bool HasCblpComponent { get; set; }
        public string StartViewName { get; set; }//only for MVI

    }

}
