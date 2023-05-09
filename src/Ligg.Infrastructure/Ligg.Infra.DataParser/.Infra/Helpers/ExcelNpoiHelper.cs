using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Text;
using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;

namespace Ligg.Infrastructure.Helpers
{
    internal static partial class ExcelHelper
    {
        //*func
        //##convert
        private static DataTable ExcelToDataTable(string filePath, string sheetName = null, bool hasColumn = true, short startRowIndex = 0, short startColumnIndex = 0, short rowQty = 0, short columnQty = 0)
        {
            IWorkbook workbook = GetWorkbook(filePath);
            ISheet sheet = GetWorkbookSheet(workbook, sheetName);
            return GetDataTableBySheet(sheet, hasColumn, startRowIndex, startColumnIndex, rowQty, columnQty);
        }

        private static DataTable GetDataTableBySheet(ISheet sheet, bool hasColumn = true, short startRowIndex = 0, short startColumnIndex = 0, short rowQty = 0, short columnQty = 0)
        {
            var dt = new DataTable();
            List<int> columnNos = new List<int>();

            int startRowNum = startRowIndex == 0 ? sheet.FirstRowNum : startRowIndex;
            IRow firstRow = sheet.GetRow(startRowNum);
            int firstRowStartColumnNum = startColumnIndex == 0 ? firstRow.FirstCellNum : startColumnIndex;
            int firstRowLastColumnNum = columnQty == 0 ? firstRow.LastCellNum - 1 : firstRowStartColumnNum + columnQty - 1;

            //column
            var i = 1;
            for (var colIndex = firstRowStartColumnNum; colIndex <= firstRowLastColumnNum; colIndex++)
            {
                if (hasColumn)
                {

                    object obj = GetCellValue(firstRow.GetCell(colIndex));
                    var val = ObjectHelper.ParseToString(obj);
                    if (string.IsNullOrEmpty(val))
                    {
                        dt.Columns.Add(new DataColumn("Column" + i.ToString()));
                    }
                    else dt.Columns.Add(new DataColumn(val));
                    i++;
                }
                else
                {
                    dt.Columns.Add(new DataColumn());
                }

                columnNos.Add(colIndex);

            }

            int dataRowStartNum = hasColumn ? (startRowNum + 1) : startRowNum;
            int dataRowEndNum = rowQty == 0 ? sheet.LastRowNum : dataRowStartNum + rowQty - 1;
            //data
            for (int rowIndex = dataRowStartNum; rowIndex <= dataRowEndNum; rowIndex++)
            {
                DataRow dr = dt.NewRow();
                bool hasValue = false;
                var row = sheet.GetRow(rowIndex);
                var drIndex = 0;
                foreach (int colIndex in columnNos)
                {
                    ICell cell = cell = row.GetCell(colIndex);
                    var valObj = GetCellValue(cell);
                    var val = ObjectHelper.ParseToString(valObj);
                    dr[drIndex] = valObj;
                    if (!string.IsNullOrEmpty(val))
                    {
                        hasValue = true;
                    }

                    drIndex++;
                }
                if (hasValue)
                {
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }
        private static IWorkbook GetWorkbook(string filePath)
        {
            if (!File.Exists(filePath)) throw new Exception("File doesn't exist");
            string postfix = Path.GetExtension(filePath).ToLower();
            IWorkbook workbook = null;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (postfix == ".xlsx") workbook = new XSSFWorkbook(fs);
                else if (postfix == ".xls") workbook = new HSSFWorkbook(fs);
                else throw new Exception("file postfix should be \".xlsx\" or \".xls\"");
                if (workbook == null) throw new Exception("Can't get proper workbook by the provided Excel file");
                return workbook;
            }
        }

        private static ISheet GetWorkbookSheet(IWorkbook workbook, string sheetName)
        {
            ISheet sheet = null;
            if (!string.IsNullOrEmpty(sheetName))
            {
                sheet = workbook.GetSheet(sheetName);
                if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                {
                    sheet = workbook.GetSheetAt(0);
                }
            }
            else sheet = workbook.GetSheetAt(0);
            if (sheet == null) throw new Exception("Can't get proper sheet by the provided Excel file");
            return sheet;
        }

        private static object GetCellValue(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:  
                    return cell.NumericCellValue;
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:  
                default:
                    return "=" + cell.CellFormula;
            }
        }

        internal enum ExcelOperation
        {
            Export,
            Insert,
            Modify
        }



    }
}
