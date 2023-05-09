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

        //*do
        public static string SaveContentToTextFile(string filePath, string ctt, bool append = false, Encoding encoding = null)
        {
            CreateDirBeforeSave(filePath);

            filePath = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileName(filePath).ToLegalFileName();
            if (append)
            {
                if (encoding != null)
                    File.AppendAllText(filePath, ctt, encoding);
                else
                {
                    File.AppendAllText(filePath, ctt);
                    //following is ok
                    //using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8))
                    //{
                    //    sw.Write(ctt);
                    //}
                }
            }
            else
            {
                if (encoding != null)
                    File.WriteAllText(filePath, ctt, encoding);
                else
                {
                    File.WriteAllText(filePath, ctt);
                    //following is ok
                    //using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
                    //{
                    //    sw.Write(ctt);
                    //}
                }
            }
            return filePath;
        }

        public static void CreateDirBeforeSave(string filePath)
        {
            var dir = GetFileDetailByOption(filePath, FilePathComposition.Directory);
            if (System.IO.Directory.Exists(dir)) return;
            CheckPathBeforeSaveAs(filePath);
            Directory.CreateDirectory(dir);
        }


        public static string Backup(string filePath, string newFilePath = null)
        {
            if (!File.Exists(filePath)) return string.Empty;
            if (newFilePath.IsNullOrEmpty())
            {
                newFilePath = Path.GetDirectoryName(filePath) + "//" + Path.GetFileNameWithoutExtension(filePath) + "-bak-" + DateTime.Now.ToString("yyMMddHHmmssfff") + Path.GetExtension(filePath);
            }

            if (File.Exists(newFilePath))
            {
                newFilePath = Path.GetDirectoryName(newFilePath) + "//" + Path.GetFileNameWithoutExtension(newFilePath) + "-bak-" + DateTime.Now.ToString("yyMMddHHmmssfff") + Path.GetExtension(newFilePath);
            }
            Copy(filePath, newFilePath,false);
            return newFilePath;
        }

        public static string Copy(string filePath, string newFilePath, bool backup)
        {
            if (filePath.ToLower() == newFilePath.ToLower()) return string.Empty;
            if(!File.Exists(filePath)) return string.Empty;

            if (backup)
            {
                Backup(newFilePath);
            }

            CreateDirBeforeSave(newFilePath);
            try
            {
                var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var fs1 = new FileStream(newFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs.CopyTo(fs1);
                fs.Close();
                fs1.Close();

            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message + "! newFilePath=" + newFilePath);
            }

            return newFilePath;
        }



    }
}




