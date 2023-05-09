namespace Ligg.RpaDesk.Parser.DataModels
{
    public class ApplicationSetting
    {
        //*get from global setting
        public string PhythonExecPath { get; set; }

        //*update global setting
        public string HelpdeskEmail { get; set; }

        //*application setting
        public bool SupportMultiLanguages { get; set; }
        public string DefaultCulture { get; set; }
        public string DefaultLanguageCode { get; set; }
        public string IncludedLanguages { get; set; }
        public string ApplicationVersion { get; set; }

        public string ApplicationDataDir { get; set; }
        public string ApplicationLibDir { get; set; }
        public string ApplicationOutputDir { get; set; }

        public bool ShowSoftwareCoverAtStart { get; set; }
        public bool LogonAtStart { get; set; }
        public bool HasCblpComponent { get; set; }

        //public bool HasOeGetter { get; set; }
        //public bool HasOeDoerr { get; set; }
        //public bool HasOeValidator { get; set; }
        //public string BootTasks { get; set; }
        //public string OessComponents { get; set; }
        //public string LogonMode { get; set; }
        //public bool VerifyPasswordAtStart { get; set; }
        //public string PasswordVerificationRule { get; set; }
        //public string StyleSheetCode { get; set; } //only for winform
        //public string StartTask { get; set; } 

    }
}
