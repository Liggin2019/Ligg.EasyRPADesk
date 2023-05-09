using System;
using System.IO;
using System.Web;
using System.Diagnostics;
using System.Linq;

namespace Ligg.Infrastructure.Helpers
{

    public static class PathHelper
    {

        public static string GetMainModuleFileName()
        {
            return Process.GetCurrentProcess().MainModule.FileName;

        }



 
        public static string GetLastDriveName()
        {

            var drivers = DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Fixed).ToList();
            var n = drivers.Count - 1;
            var str = drivers[n].Name;
            return DirectoryHelper.RemoveLastSlashes(str);

        }


    }
}
