using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Data;


namespace Ligg.Infrastructure.Utility.DataParser
{
    public static class ConfigFileHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        internal static string[] Postfixes = new string[] { ".json", ".xml", ".csv", ".xlsx", ".xls" };
        public static bool IsFileExisting(string filePath)
        {
            if (filePath.IsNullOrEmpty()) return false;
            var postfix = FileHelper.GetFileDetailByOption(filePath, FilePathComposition.Postfix).ToLower();
            if (postfix.IsNullOrEmpty())
            {

                var dir = FileHelper.GetFileDetailByOption(filePath, FilePathComposition.Directory);
                var fileTitle = FileHelper.GetFileDetailByOption(filePath, FilePathComposition.FileTitle);
                foreach (var postfix1 in ConfigFileHelper.Postfixes)
                {
                    if (File.Exists(dir + "\\" + fileTitle + postfix1))
                    {
                        return true;
                    }
                }
            }
            else if (Postfixes.Any(x => x.Equals(postfix)))
            {
                return File.Exists(filePath);
            }
            return false;
        }


        internal static T ConvertToGeneric<T>(string filePath, string tableName)
        {
            var postfix = FileHelper.GetFileDetailByOption(filePath, FilePathComposition.Postfix);
            postfix = postfix.ToLower();

            try
            {
                if (postfix == ".json")
                {
                    var str = GetContentFromTextFileEx(filePath, postfix);
                    return JsonHelper.ConvertToGeneric<T>(str);
                }
                else if (postfix == ".xml")
                {
                    var xmlStr = GetContentFromTextFileEx(filePath, postfix);
                    return XmlHelper.ConvertToGeneric<T>(xmlStr);
                }
                else if (postfix == ".csv")
                {
                    var str = GetContentFromTextFileEx(filePath, postfix);
                    var dt = CsvHelper.ConvertToDataTable(str);
                    var isEntityFormat = dt.IsEntityFormat();
                    var json = dt.ConvertToJsonEx(isEntityFormat);
                    return JsonHelper.ConvertToGeneric<T>(json);
                }
                else if (postfix == ".xlsx" | postfix == ".xls")
                {
                    var dt = ExcelHelper.ConvertToDataTable(filePath, tableName);
                    var isEntityFormat = dt.IsEntityFormat();
                    var json = dt.ConvertToJsonEx(isEntityFormat);
                    var obj = JsonHelper.ConvertToGeneric<T>(json);
                    return obj;
                }
                return default(T);
                //throw new NotImplementedException();

            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".ConvertToGeneric error: " + ex.Message + "; filepath= " + filePath);
            }
        }

        private static string GetContentFromTextFileEx(string filePath, string srcPostfix)
        {
            if (srcPostfix == ".csv") return FileHelper.GetContentFromTextFile(filePath, Encoding.Default);
            else return FileHelper.GetContentFromTextFile(filePath);
        }


    }

}
