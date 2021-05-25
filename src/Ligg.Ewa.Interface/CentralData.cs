using System;

namespace Ligg.EasyWinApp.Interface
{
    public static class CentralData
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //common settings: set from ReadParams() 
        public static string ArchitectureCode = "";
        public static string ArchitectureName = "";
        public static string ArchitectureVersion = "";

        public static string OrganizationCode = "";
        public static string OrganizationShortName = "";
        public static string OrganizationName = "";

        public static string GlobalKey1 = "";
        public static string GlobalKey2 = "";

        //app params: set from ui end
        public static string AppCode = "";
        public static string ApplicationLibDir = "";
        public static string ApplicationDataDir = "";
        public static string CommonParams;
        public static string PythonExecPath;
        public static bool SupportMultiCultures;
        public static string DefaultLanguageCode = "";
        public static string CurrentLanguageCode = "";


        public static string UserCode = "";
        public static string UserToken = "";


        public static void InitCommonSettings()
        {
            ArchitectureCode = "Ligg.EWA";
            ArchitectureName = "Ligg.EasyWinApp";
            ArchitectureVersion = "3.52";

            OrganizationCode = "Ligg";
            OrganizationShortName = "Ligg Tech";
            OrganizationName = "理格科技有限公司";
            GlobalKey1 = "GlobalEncrptKey1";
            GlobalKey2 = "GlobalEncrptKey2";
        }
        public static void InitApplicationSettings(string applicationCode, bool supportMutiCultures, string defaultLanguageCode, string currentLanguageCode, string commonParams, string appLibDir, string appDataDir,string pythonExecPath)
        {
            AppCode = applicationCode;
            SupportMultiCultures = supportMutiCultures;
            //DefaultLanguageCode = defaultLanguageCode;
            //CurrentLanguageCode = currentLanguageCode;

            CommonParams = commonParams;
            ApplicationLibDir = appLibDir;
            ApplicationDataDir = appDataDir;
            PythonExecPath=pythonExecPath;

        }




    }
}