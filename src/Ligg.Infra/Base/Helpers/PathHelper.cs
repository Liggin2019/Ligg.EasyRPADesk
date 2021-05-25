using System;
using System.IO;
using System.Web;
using System.Diagnostics;
using System.Linq;

namespace Ligg.Infrastructure.Base.Helpers
{

    public static class PathHelper
    {

        public static string LocateServerPath(string path)
        {
            if (System.IO.Path.IsPathRooted(path) == false)
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
                //path = System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, path);
            }
            return path;
        }

        public static string CombineUrl(string baseUrl, string relativeUrl)
        {
            if (relativeUrl.Length == 0 || relativeUrl[0] != '/')
                relativeUrl = '/' + relativeUrl;

            if (baseUrl.Length > 0 && baseUrl[baseUrl.Length - 1] == '/')
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            return baseUrl + relativeUrl;
        }

        public static string GetMainModuleFileName()
        {
            return Process.GetCurrentProcess().MainModule.FileName;

        }

        public static string GetCurrentDirectory()
        {
            return DirectoryHelper.DeleteLastSlashes(AppDomain.CurrentDomain.BaseDirectory);
            //return Directory.GetCurrentDirectory();
            //return Environment.CurrentDirectory;
            //return AppDomain.CurrentDomain.SetupInformation.ApplicationBas; //AppDomain.CurrentDomain has no SetupInformation in .net std 
        }

        // .net standard 不存在HttpContext
        private static string GetWebAppUrl()
        {
            //var request = System.Web.HttpContext.Current.Request;
            //return CombineUrl(request.Url.GetLeftPart(UriPartial.Authority), request.ApplicationPath);
            return string.Empty;
        }

        public static string GetFirstDriveName()
        {
            var drivers = DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Fixed).ToList();
            var str = drivers[0].Name;
            return DirectoryHelper.DeleteLastSlashes(str);
        }
        public static string GetLastDriveName()
        {

            var drivers = DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Fixed).ToList();
            var n = drivers.Count - 1;
            var str = drivers[n].Name;
            return DirectoryHelper.DeleteLastSlashes(str);

        }


    }
}
