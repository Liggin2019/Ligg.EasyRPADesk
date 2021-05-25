using Ligg.Infrastructure.Base.Helpers;
using System.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Ligg.Infrastructure.Base.DataModel.Enums;
using System.IO;

namespace Ligg.Infrastructure.Utility.FileWrap
{

    public static class CsvHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        public static T ConvertToGeneric<T>(string cvsPath)
        {
            try
            {
                var str = ConvertToJson(cvsPath);
                return JsonHelper.ConvertToGeneric<T>(str);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToGeneric Error: " + ex.Message);
            }
        }
        public static string ConvertToJson(string cvsPath)
        {
            try
            {
                var isEntityFormat = false;
                if (FileHelper.IsFileExisting(cvsPath+ ".IsEntity")) isEntityFormat = true;
                var dt = ConvertToDataTable(cvsPath);
                var str = DataTableHelper.ConvertToJson(dt, isEntityFormat);
                return str;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToJson Error: " + ex.Message);
            }
        }


        //#common
        private static DataTable ConvertToDataTable(string cvsPath)
        {
            try
            {
                return WeihanLi.Npoi.CsvHelper.ToDataTable(cvsPath);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToJson Error: " + ex.Message);
            }
        }


    }
}
