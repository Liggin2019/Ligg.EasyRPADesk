using System;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.DataModels;
using System.IO;


namespace Ligg.Infrastructure.Utility.DataParser
{
    public class ConfigFileManager
    {
        private string _filePath;
        private string[] _postfixes = new string[] { ".ejson", ".json", ".xml", ".csv", ".xlsx", ".xls" };
        
        //cons
        public ConfigFileManager(string filePath)
        {
            Constructor(filePath);
        }
        private void Constructor(string filePath)
        {
            var exInfo = "\n >> " + GetType().FullName + ".Constructor error: ";
            if (filePath.IsNullOrEmpty()) throw new ArgumentException(exInfo + "filePath can't be empty!");

            var postfix = FileHelper.GetFileDetailByOption(filePath, FilePathComposition.Postfix);
            if (!postfix.IsNullOrEmpty())
            {
                if (!postfix.IsBeContainedInStringArray(ConfigFileHelper.Postfixes, true)) throw new ArgumentException(exInfo + "config file postfix is incorrect; "
                + "File should have a postfix like \"{0}\"! filePath=\"{1}\"".FormatWith(StringArrayExtension.Unwrap(_postfixes, ','.ToString()), filePath));

                _filePath = filePath;
            }
            else
            {
                var dir = FileHelper.GetFileDetailByOption(filePath, FilePathComposition.Directory);
                var fileTitle = FileHelper.GetFileDetailByOption(filePath, FilePathComposition.FileTitle);
#if DEBUG
                _postfixes = new string[] { ".xlsx", ".xls", ".csv", ".xml", ".json", ".ejson" };
#endif
                foreach (var postfix1 in ConfigFileHelper.Postfixes)
                {
                    if (File.Exists(dir + "\\" + fileTitle + postfix1))
                    {
                        _filePath = dir + "\\" + fileTitle + postfix1;
                        break;
                    }
                }
            }

        }


        public T ConvertToGeneric<T>(string tableName = null)
        {
            var obj = ConfigFileHelper.ConvertToGeneric<T>(_filePath,tableName);
            return obj;
        }


    }
}



