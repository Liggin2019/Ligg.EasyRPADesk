using Ligg.Infrastructure.Base.Helpers;
using System.Data;
using System;
using System.IO;
using Ligg.Infrastructure.Base.DataModel.Enums;


namespace Ligg.Infrastructure.Utility.FileWrap
{

    public static class ExcelHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static T ConvertToGeneric<T>(string excelPath)
        {
            try
            {
                var str = ConvertToJson(excelPath);
                return JsonHelper.ConvertToGeneric<T>(str);

            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToGeneric Error: " + ex.Message);
            }
        }
        public static string ConvertToJson(string excelPath)
        {
            try
            {
                var workbook = WeihanLi.Npoi.ExcelHelper.LoadExcel(excelPath);
                var sheetName = workbook.GetSheetName(0);
                var isEntityFormat = false;
                if (sheetName.ToLower().Contains("entity")) isEntityFormat = true;
                var dt = ConvertToDataTable(excelPath);
                var str = DataTableHelper.ConvertToJson(dt, isEntityFormat);
                return str;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToJson Error: " + ex.Message);
            }
        }

        public static string ConvertToJson(string excelPath, TernaryOption includeOrExlude, string[] fieldArry)
        {
            try
            {
                var dt = ConvertToDataTable(excelPath);
                var isEntityFormat = false;
                if (dt.TableName.ToLower().Contains("entity")) isEntityFormat = true;
                var dt1 = DataTableHelper.Shrink(dt, includeOrExlude, fieldArry);
                return DataTableHelper.ConvertToJson(dt1, isEntityFormat);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToJson Error: " + ex.Message);
            }
        }





        //#common
        private static DataTable ConvertToDataTable(string excelPath)
        {
            try
            {
                return WeihanLi.Npoi.ExcelHelper.ToDataTable(excelPath);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToDataTable Error: " + ex.Message);
            }

        }


    }
}
