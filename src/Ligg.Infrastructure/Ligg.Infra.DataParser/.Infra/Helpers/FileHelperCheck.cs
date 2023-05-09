using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using System;

namespace Ligg.Infrastructure.Helpers
{
    internal static partial class FileHelper
    {
        //*check

        private static void CheckPathBeforeSaveAs(string filePath)
        {
            CheckPathLegality(filePath);
            var dir = GetFileDetailByOption(filePath, FilePathComposition.Directory);
            //DirectoryHelper.CheckPathVadility(dir);
        }


        internal static void CheckPathLegality(string path)
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
        internal static void CheckFileNameLegality(string fileName)
        {
            if (!fileName.IsLegalFileName()) throw new ArgumentException("CheckFileNameLegality error: " + "File Name is illegal! ");
        }

    }
}





