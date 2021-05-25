using System.Linq;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.DataModel.Enums;

namespace Ligg.Infrastructure.Utility.FileWrap
{

    public static class ConfigFileHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static string[] Postfixes = new string[] { ".xlsx", ".csv", ".xml" };

        public static bool IsFileExisting(string filePath)
        {
            if (filePath.IsNullOrEmpty()) return false;
            var postfix = FileHelper.GetFileDetailByOption(filePath, FilePathComposition.Postfix).ToLower();
            if (Postfixes.Any(x => x.Equals(postfix)))
            {
                return FileHelper.IsFileExisting(filePath);
            }
            else
            {
                var dir = FileHelper.GetFileDetailByOption(filePath, FilePathComposition.Directory);
                var fileTitle = FileHelper.GetFileDetailByOption(filePath, FilePathComposition.FileTitle);
                if (FileHelper.IsFileExisting(dir + "\\" + fileTitle + Postfixes[0])) return true;
                if (FileHelper.IsFileExisting(dir + "\\" + fileTitle + Postfixes[1])) return true;
                if (FileHelper.IsFileExisting(dir + "\\" + fileTitle + Postfixes[2])) return true;
            }
            return false;
        }
    }

}
