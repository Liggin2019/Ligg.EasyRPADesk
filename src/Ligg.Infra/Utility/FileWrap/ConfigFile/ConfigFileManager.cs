using System;
using System.IO;
using System.Data;
using System.Text;
using System.Xml.Serialization;
using Ligg.Infrastructure.Base;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.DataModel.Enums;
using System.Collections.Generic;

namespace Ligg.Infrastructure.Utility.FileWrap
{
    public class ConfigFileManager
    {
        private string _filePath;
        //cons
        public ConfigFileManager(string filePath)
        {
            var exInfo = "\n >> " + GetType().FullName + ".Constructor error: ";

            if (filePath.IsNullOrEmpty()) throw new ArgumentException(exInfo+"filePath can't be empty!");
            var postfix = FileHelper.GetFileDetailByOption(filePath, FilePathComposition.Postfix);
            if (postfix.IsBeContainedInStringArray(ConfigFileHelper.Postfixes))
            {
                FileHelper.CheckFilePathExistence(filePath);
                _filePath = filePath;
                return;
            }

            if (!postfix.IsNullOrEmpty())
            {
                throw new ArgumentException(exInfo+"File should have a postfix like \"{0}\"! filePath=\"{1}\"".FormatWith(StringHelper.UnwrapStringArrayBySeparator(ConfigFileHelper.Postfixes, ','), filePath));
            }

            var dir = FileHelper.GetFileDetailByOption(filePath, FilePathComposition.Directory);
            var fileTitle = FileHelper.GetFileDetailByOption(filePath, FilePathComposition.FileTitle);

            if (FileHelper.IsFileExisting(dir + "\\" + fileTitle + ConfigFileHelper.Postfixes[0])) _filePath = dir + "\\" + fileTitle + ConfigFileHelper.Postfixes[0];
            else if (FileHelper.IsFileExisting(dir + "\\" + fileTitle + ConfigFileHelper.Postfixes[1])) _filePath = dir + "\\" + fileTitle + ConfigFileHelper.Postfixes[1];
            else if (FileHelper.IsFileExisting(dir + "\\" + fileTitle + ConfigFileHelper.Postfixes[2])) _filePath = dir + "\\" + fileTitle + ConfigFileHelper.Postfixes[2];

            else { throw new ArgumentException(exInfo + "pls check config file \"{0}\"is valid! ".FormatWith(filePath)); }
            
        }


        public T ConvertToGeneric<T>()
        {
            try
            {
                //Postfixes = new string[] { ".xlsx", ".csv", ".xml", ".json", ".ejson" };
                if (_filePath.ToLower().EndsWith(ConfigFileHelper.Postfixes[0]))
                {
                    return ExcelHelper.ConvertToGeneric<T>(_filePath);
                }
                else if (_filePath.ToLower().EndsWith(ConfigFileHelper.Postfixes[1]))
                {
                    return CsvHelper.ConvertToGeneric<T>(_filePath);
                }
                else if (_filePath.ToLower().EndsWith(ConfigFileHelper.Postfixes[2]))
                {
                    var xmlStr = FileHelper.GetContentFromTextFile(_filePath);
                    return XmlHelper.ConvertToGeneric<T>(xmlStr);
                }
                else if (_filePath.ToLower().EndsWith(ConfigFileHelper.Postfixes[3]))
                {
                    throw new NotImplementedException();
                }

                throw new NotImplementedException();

            }
            catch (Exception ex)
            {
                throw new ArgumentException(("\n>> " + GetType().FullName + ".ConvertToGeneric error: " + ex.Message +"; filepath=\"{0}\"").FormatWith(_filePath)); 
            }
        }



    }
}



