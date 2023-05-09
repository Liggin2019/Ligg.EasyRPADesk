using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using System;

namespace Ligg.Infrastructure.Helpers
{
    public static partial class FileHelper
    {
        //*check

        public static void CheckPathExistence(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new ArgumentException("CheckPathExistence error: " + "File does not exist! filePath=" + filePath);
            }
        }

        private static void CheckPathBeforeSaveAs(string filePath)
        {
            CheckPathLegality(filePath);
            var dir = GetFileDetailByOption(filePath, FilePathComposition.Directory);
            //DirectoryHelper.CheckPathVadility(dir);
        }


        public static void CheckPathLegality(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException( "CheckPathLegality error: " + "FilePath can't be null! ");
            }

            var fileName = GetFileDetailByOption(path, FilePathComposition.FileName);
            CheckFileNameLegality(fileName);

            var dir = GetFileDetailByOption(path, FilePathComposition.Directory);
            DirectoryHelper.CheckPathLegality(dir);
        }
        public static void CheckFileNameLegality(string fileName)
        {
            if (!fileName.IsLegalFileName()) throw new ArgumentException("CheckFileNameLegality error: " + "File Name is illegal! ");
        }

    }
}





