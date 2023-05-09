using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Ligg.Infrastructure.Helpers
{
    public static partial class FileHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //*get
        public static string GetPath(string path, string locaton, bool checkPathIsRelativePath = false)
        {
            
            if (!DirectoryHelper.IsLegalAbsoluteDirectory(locaton)) throw new ArgumentException(_typeFullName + ".GetPath error: " + "Location is illegal absolute directory, locaton= " + locaton);
            //if (path.IsNullOrEmpty()) return "";
            if (path.IsNullOrEmpty()) return locaton;
            if (checkPathIsRelativePath)
            {
                if (path.StartsWith("~"))
                {
                    throw new ArgumentException(_typeFullName + ".GetPath error: " + "Relative path can not begins with '~'");
                }
                else if (path.StartsWith("\\\\"))
                {
                    throw new ArgumentException(_typeFullName + ".GetPath error: " + "Relative path can not begins with '\\\\'");
                }
                else if (path.Contains(":"))
                {
                    throw new ArgumentException(_typeFullName + ".GetPath error: " + "Relative path can not contains ':'");
                }
            }

            locaton = DirectoryHelper.RemoveLastSlashes(locaton);
            var rst = "";
            if (path.StartsWith("~"))
            {
                var dir = Directory.GetCurrentDirectory();
                rst = path.Replace("~", dir);
            }
            if (path.IsLegalAbsolutePath())
            {
                rst = path;
            }
            else if (path.StartsWith("\\"))
            {
                rst = locaton + path;
            }
            else
            {
                rst = locaton + "\\" + path;
            }
            return rst;
        }


        public static string GetFileDetailByOption(string path, FilePathComposition returnOpt)
        {

            var dir = Path.GetDirectoryName(path);
            var fileName = Path.GetFileName(path);
            var fileTitle = Path.GetFileNameWithoutExtension(path);
            var postfix = Path.GetExtension(path);

            if (returnOpt == FilePathComposition.FilePathTitle)
            {
                return Path.Combine(dir, fileTitle);
            }
            else if (returnOpt == FilePathComposition.Directory) return dir;
            else if (returnOpt == FilePathComposition.Folder)
            {
                var arr = dir.Split('\\');
                return arr[arr.Length - 1];
            }
            else if (returnOpt == FilePathComposition.FileName) return fileName; //file name
            else if (returnOpt == FilePathComposition.FileTitle)
            {
                return fileTitle;
            }
            else if (returnOpt == FilePathComposition.Postfix)
            {
                return postfix;
            }
            else if (returnOpt == FilePathComposition.Suffix)
            {
                var surfix = "";
                if (!postfix.IsNullOrEmpty()) surfix = postfix.Substring(1, postfix.Length - 1);
                return surfix;
            }
            return string.Empty;
        }


        public static string GetContentFromTextFile(string filePath, Encoding encoding = null)
        {
            if (!File.Exists(filePath)) return string.Empty;
            if (File.Exists(filePath))
            {
                try
                {
                    if (encoding == null) return File.ReadAllText(filePath);
                    else return File.ReadAllText(filePath, encoding);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(_typeFullName + ".GetContentFromTxtFile error: " + ex.Message);
                }
            }
            return string.Empty;
        }

 
    }
}





