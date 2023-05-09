using System;
using System.Collections.Generic;

namespace Ligg.RpaDesk.Interface
{
    public static class CentralData
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static string ArchitectureCode = "";
        public static string ArchitectureName = "";
        public static string ArchitectureVersion = "";

        public static string OrganizationCode = "";
        public static string OrganizationShortName = "";
        public static string OrganizationName = "";

        public static string AppCode = "";
        public static string ApplicationLibDir = "";
        public static string ApplicationDataDir = "";
        public static string ApplicationOutputDir = "";


        //*common
        public static string PythonExecPath;
        public static bool SupportMultiCultures;
        public static string DefaultLanguageCode = "";
        public static string CurrentLanguageCode = "";

        public static string InitParams;
        public static string UserId = "";
        public static string UserAccount = "";
        public static string UserName = "";

        public static Dictionary<String, string> ExchangeData = new Dictionary<string, string>();
        //public static Dictionary<String, string> Constants = new Dictionary<string, string>();

        public static void InitCommonSetting()
        {
            ArchitectureCode = "LRD";
            ArchitectureName = "Ligg.RpaDesk";
            ArchitectureVersion = "4.3.7";

            OrganizationCode = "Ligg";
            OrganizationShortName = "Ligg Tech";
            OrganizationName = "Ligg Tech Co. Ltd.";
        }
        public static void InitApplicationSetting(string applicationCode, bool supportMutiCultures, string currentLanguageCode, string appLibDir, string appDataDir, string appTempDataDir, string pythonExecPath)
        {
            AppCode = applicationCode;
            SupportMultiCultures = supportMutiCultures;
            DefaultLanguageCode = currentLanguageCode;
            CurrentLanguageCode = currentLanguageCode;

            ApplicationLibDir = appLibDir;
            ApplicationDataDir = appDataDir;
            ApplicationOutputDir = appTempDataDir;
            PythonExecPath = pythonExecPath;
        }

    }
}