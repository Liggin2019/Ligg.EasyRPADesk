using System;
using System.IO;
using System.Linq;
using Ligg.Infrastructure.Base.DataModel.Enums;
using Ligg.Infrastructure.Base.Extension;

namespace Ligg.Infrastructure.Base.Helpers
{
    public static class FileHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //#add
        public static void SaveContentToTextFile(string ctt, string filePath, bool append)
        {

            if (append)
            {
                CreateDirsBeforeSave(filePath);
                File.AppendAllText(filePath, ctt);
            }
            else
            {
                CreateDirsBeforeSave(filePath);
                File.WriteAllText(filePath, ctt);
            }
        }

        public static void CreateDirsBeforeSave(string filePath)
        {
            var dir = GetFileDetailByOption(filePath, FilePathComposition.Directory);
            if (System.IO.Directory.Exists(dir)) return;
            CheckFilePathBeforeSaveAs(filePath);
            Directory.CreateDirectory(dir);

        }

        //#del
        public static void BatchDelete(string absoluteLocation, string[] fileNames)
        {
            foreach (var v in fileNames)
            {
                System.IO.File.Delete(absoluteLocation + "/" + v);
            }
        }

        //#mod
        public static void BatchMove(string formAbsoluteLocation, string toAbsoluteLocation, string[] fileNames)
        {
            foreach (var v in fileNames)
            {
                System.IO.File.Move(formAbsoluteLocation + "/" + v, toAbsoluteLocation + "/" + v);
            }
        }



        //#get
        public static string GetFilePath(string url, string defaultLocaton)
        {
            var retStr = "";

            if (url.StartsWith("~"))
            {
                var dir = Directory.GetCurrentDirectory();
                retStr = url.Replace("~", dir);
            }
            else if (url.StartsWith("\\\\"))
            {
                retStr = url;
            }
            else if (url.Contains(":"))
            {
                retStr = url;
            }
            else if (url.StartsWith("\\"))
            {
                retStr = defaultLocaton + url;
            }
            else
            {
                retStr = defaultLocaton + "\\" + url;
            }
            return retStr;
        }

        public static string GetFilePathByRelativeLocation(string url, string defaultLocaton)
        {
            var retStr = "";

            if (url.StartsWith("~"))
            {
                throw new ArgumentException("Relative location can not begins with '~'");
            }
            else if (url.StartsWith("\\\\"))
            {
                throw new ArgumentException("Relative location can not begins with '\\\\'");
            }
            else if (url.Contains(":"))
            {
                throw new ArgumentException("Relative location can not contains ':'");
            }
            else if (url.StartsWith("\\"))
            {
                retStr = defaultLocaton + url;
            }
            else
            {
                retStr = defaultLocaton + "\\" + url;
            }
            return retStr;
        }

        public static string GetLegalFileName(string fileName)
        {
            if (!fileName.Contains("."))
            {
                throw new ArgumentException("File name must contains suffix seperator .");
            }
            var fileNameArray = fileName.Split('.');
            var fileNamePostfix = (fileNameArray[fileNameArray.Length - 1]).ToLower();

            var fileNamePrefix = fileName.Substring(0, fileName.Length - fileNamePostfix.Length - 1);

            fileNamePrefix = fileNamePrefix.ToLegalUrl();
            fileName = fileNamePrefix + "." + fileNamePostfix;

            return fileName;
        }


        public static string GetFileDetailByOption(string path, FilePathComposition returnOpt)
        {
            int lastIndex = path.LastIndexOf("\\");
            var dir = path.Substring(0, lastIndex);
            var fileName = path.Substring(lastIndex + 1, path.Length - lastIndex - 1);
            if (returnOpt == FilePathComposition.Directory) return dir;//dir
            else if (returnOpt == FilePathComposition.FileName) return fileName; //file name

            else if (returnOpt == FilePathComposition.FileTitle)
            {
                if (fileName.Contains('.'))
                    return GetFilePathOrNameDetailByOption(path, FilePathComposition.FileTitle);
                else
                {
                    return fileName;
                }
            }
            else if (returnOpt == FilePathComposition.Postfix)
            {
                if (fileName.Contains('.')) return GetFilePathOrNameDetailByOption(path, FilePathComposition.Postfix);
                else return string.Empty;
            }
            else if (returnOpt == FilePathComposition.Suffix) ;
            {
                if (fileName.Contains('.')) return GetFilePathOrNameDetailByOption(path, FilePathComposition.Suffix);
                else return string.Empty;
            }
            return string.Empty;
        }

        private static string GetFilePathOrNameDetailByOption(string filePathOrName, FilePathComposition returnOpt)  //1:fileTitle;2:postfix;3:suffix;
        {
            int lastIndexOfDot = filePathOrName.LastIndexOf(".");
            if (lastIndexOfDot == -1)
            {
                throw new ArgumentException("Filename must contains separator  '.'");
            }
            string fileTitle = filePathOrName.Substring(0, lastIndexOfDot);
            int lastIndexOfDoubleSlash = fileTitle.LastIndexOf("\\");
            if (lastIndexOfDoubleSlash != -1)
            {
                fileTitle = fileTitle.Substring(lastIndexOfDoubleSlash + 1, fileTitle.Length - lastIndexOfDoubleSlash - 1);
            }
            string suffix = filePathOrName.Substring(lastIndexOfDot + 1, filePathOrName.Length - lastIndexOfDot - 1);
            string postfix = "." + suffix;
            if (returnOpt == FilePathComposition.FileTitle)
            {
                return fileTitle;
            }
            else if (returnOpt == FilePathComposition.Postfix)
            {
                return postfix;
            }
            else if (returnOpt == FilePathComposition.Suffix)
            {
                return suffix;
            }
            else
            {
                return string.Empty;
            }
        }

        //outputOpt 1: KB; 2:GB,MB,KB,B

        public static string GetContentFromTextFile(string filePath)
        {
            CheckFilePathExistence(filePath);

            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".GetContentFromTxtFile Error: " + ex.Message);
            }
        }

        /*
        //function is not OK, need to check. error: 在dll里 call ,not ok. same code in form cs is OK.
        //System.Drawing.Icon not exsit in .net standard 2.0
        //ok in listviewEx
        //http://blog.csdn.net/wyh0318/article/details/8060620 获取系统文件图标
        [DllImport("Shell32.dll")]
        static extern int SHGetFileInfo(string filePath, uint fileAttributes, ref ShellFileInfo shellFileInfo, uint cbFileInfo, uint uFlags);
        struct ShellFileInfo
        {
            public IntPtr iconIntPtr;
            public int iconInt;
            public uint attributes;
            public char displayName;
            public char typeName;
        }

        static public System.Drawing.Icon GetFileIcon(string fileName, bool isSmallIcon)
        {
            var shellFileInfo = new ShellFileInfo();
            Icon icon = null;
            var total = (int)SHGetFileInfo(fileName, 100, ref shellFileInfo, 0, (uint)(isSmallIcon ? 273 : 272));
            if (total > 0)
            {
                icon = Icon.FromHandle(shellFileInfo.iconIntPtr);
            }
            return icon;
        }
        */

        //output options: -2: file2 not exist,-1: file1 not exis; 0: same, 1: file1 new, 2: file 2 new
        public static int Compare2Files(string filePath1, string filePath2)
        {
            if (!System.IO.File.Exists(filePath2))
            {
                return -2;
            }
            if (!System.IO.File.Exists(filePath1))
            {
                return -1;
            }

            var fi1 = new System.IO.FileInfo(filePath1);
            var fi2 = new System.IO.FileInfo(filePath2);
            var sizeOfFile1 = fi1.Length;
            var lastWriteTimeOfFile1 = fi1.LastWriteTime;
            var sizeOfFile2 = fi2.Length;
            var lastWriteTimeOfFile2 = fi2.LastWriteTime;
            if (sizeOfFile1 != sizeOfFile2)
            {
                if (lastWriteTimeOfFile1 > lastWriteTimeOfFile2)
                { return 1; }
                else { return 2; }
            }
            return 0;
        }



        //#judge
        public static bool IsFileExisting(string filePath)
        {
            if (filePath.IsNullOrEmpty()) return false;
            return File.Exists(filePath);
        }
        public static bool IsAbsolutePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }
            if (!filePath.IsLegalFileName())
            {
                return false;
            } 
            var pathFirst2Letters = filePath.Substring(0, 2);
            if (pathFirst2Letters == "\\\\")
            {
                if (filePath.Contains(":"))
                {
                    return false;
                }
            }
            else
            {
                if (!filePath.Contains(":"))
                {
                    return false;
                }
            }

            return true;
        }


        public static bool IsValidImageExt(string imageExt)
        {
            var allowExt = new string[] { ".gif", ".jpg", ".jpeg", ".bmp", ".png", ".tif" };
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            return allowExt.Any(c => stringComparer.Equals(c, imageExt));
        }
        public static bool IsFileNameLegal(string name)
        {
            if (name.IsLegalFileName()) return true; else return false;
        }

        //#check
        public static void CheckFileNameLegality(string fileName)
        {
            if (!fileName.IsLegalFileName()) throw new ArgumentException("File Name is illegal! ");
        }

       
        public static void CheckFilePathLegality(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("FilePath can't be null! ");
            }
            var fileName = GetFileDetailByOption(filePath, FilePathComposition.FileName);
            CheckFileNameLegality(fileName);

            var pathFist2Letters = filePath.Substring(0, 2);
            if (pathFist2Letters == "\\\\")
            {
                if (filePath.Contains(":"))
                {
                    throw new ArgumentException("FilePath should not contains both '\\' and ':', ! filePath=" + filePath);
                }
            }
            else
            {
                if (!filePath.Contains(":"))
                {
                    throw new ArgumentException("FilePath should contains ':', ! filePath=" + filePath);
                }
            }

        }

        public static void CheckFilePathPostfix(string filePath)
        {
            CheckFilePathLegality(filePath);
            int lastIndexOfDot = filePath.LastIndexOf(".");
            if (true)
            {
                if (lastIndexOfDot == -1)
                {
                    throw new ArgumentException("FilePath must end with postfix, with separator  '.'! filePath=" + filePath);
                }
            }
        }

        public static void CheckFilePathExistence(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new ArgumentException("File does not exist! filePath=" + filePath);
            }
        }

        private static void CheckFilePathBeforeSaveAs(string filePath)
        {
            CheckFilePathLegality(filePath);

            var disk = "";
            var array = filePath.Split('\\');
            var pathFist2Letters = filePath.Substring(0, 2);
            if (pathFist2Letters == "\\\\")
            {
                disk = "\\\\" + array[2];
            }
            else
            {
                disk = array[0];
            }
            if (!Directory.Exists(disk))
            {
                throw new ArgumentException("the filePath disk: \"{0}\" does not exsit! ; filePath= \"{1}\"".FormatWith(disk, filePath));
            }

        }





    }
}