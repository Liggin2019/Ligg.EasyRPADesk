namespace Ligg.EasyWinApp
{
    public class ApplicationSetting
    {
        //#get from global setting
        public string BootStrapperTasksDllPath { get; set; }
        public string BootStrapperTaskNameSpace { get; set; }
        public string PhythonExecPath { get; set; }

        //#update global setting
        public string HelpdeskEmail { get; set; }
        public bool SupportMultiCultures { get; set; }
        public string DefaultCulture { get; set; }


        //#application setting
        public string ApplicationVersion { get; set; }

        public bool RunBootStrapperTasksAtStart { get; set; }
        public string BootStrapperTasks { get; set; }

        public bool HasCblpComponent { get; set; }
        public string CblpDllPath { get; set; }
        public string CblpAdapterClassFullName { get; set; }

        public string ApplicationDataDir { get; set; }
        public string ApplicationLibDir { get; set; }
        public string ApplicationTempDir { get; set; }



        public bool VerifyPasswordAtStart { get; set; }
        public string PasswordVerificationRule { get; set; }


        public bool ShowSoftwareCoverAtStart { get; set; }
        public string SoftwareCoverLocation { get; set; }

        public bool LogonAtStart { get; set; }
        public string LogonLocation { get; set; }

        //# following 1 only for winform
        public string StyleSheetCode { get; set; }

        //# following 1 only for console
        public string Invisible { get; set; }



    }


}
