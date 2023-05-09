
using Ligg.Infrastructure.Extensions;
using System;
using System.IO;

namespace Ligg.Infrastructure.Helpers
{
    internal static class DirectoryHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //*get
        internal static string GetDirveName(string path)
        {
            if (!path.IsLegalAbsoluteDirectory()) return string.Empty;

            var array = path.Split('\\');
            var pathFist2Letters = path.Substring(0, 2);

            string dirve;
            if (pathFist2Letters == "\\\\")
            {
                dirve = "\\\\" + array[2] + "\\" + array[3];
            }
            else
            {
                dirve = array[0];
            }
            return dirve;
        }

        internal static int GetRecursiveSubFileNo(string dir)
        {
            if (!Directory.Exists(dir))
            {
                throw new ArgumentException("Directory: " + dir + " does not exsit!");
            }
            var no = Directory.GetFiles(dir).Length;
            var subDirs = Directory.GetDirectories(dir);
            foreach (var subDir in subDirs)
            {
                no = no + GetRecursiveSubFileNo(subDir);
            }
            return no;
        }


        //*judge
        internal static bool IsLegalAbsoluteDirectory(this string target)
        {
            if (string.IsNullOrEmpty(target)) return false;

            if (!target.IsLegalDirectory()) return false;
            if (target.Length < 2) return false;

            if (target.Length > 2)
            {
                var pathFirst3Letters = target.Substring(0, 3);
                if (pathFirst3Letters == "\\\\\\") return false;
            }

            var pathFirst2Letters = target.Substring(0, 2);
            if (pathFirst2Letters == "\\\\")
            {
                if (target.Contains(":")) return false;
            }
            else if (!target.Contains(":")) return false;

            return true;
        }


        internal static bool HasRecursiveSubFile(string dir)
        {
            if (!Directory.Exists(dir)) return false;

            if (Directory.GetFiles(dir).Length > 0) return true;
            var subDirs = Directory.GetDirectories(dir);
            foreach (var subDir in subDirs)
            {
                return HasRecursiveSubFile(subDir);
            }
            return false;
        }


        //*check
        internal static void CheckPathLegality(string dir)
        {
            if (!dir.IsLegalAbsoluteDirectory()) throw new ArgumentException(_typeFullName + ".CheckPathLegality error: " + "directory is illegal! dir=" + dir);
        }


    }
}
