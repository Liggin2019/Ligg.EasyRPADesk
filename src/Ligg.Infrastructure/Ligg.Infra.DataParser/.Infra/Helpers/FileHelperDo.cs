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
    internal static partial class FileHelper
    {
        //*do
        internal static void CreateDirBeforeSave(string filePath)
        {
            var dir = GetFileDetailByOption(filePath, FilePathComposition.Directory);
            if (System.IO.Directory.Exists(dir)) return;
            CheckPathBeforeSaveAs(filePath);
            Directory.CreateDirectory(dir);
        }

        internal static string Backup(string filePath, string newFilePath = null)
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

        internal static string Copy(string filePath, string newFilePath, bool backup)
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




