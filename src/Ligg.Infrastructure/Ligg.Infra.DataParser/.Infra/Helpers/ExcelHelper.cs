using System.Data;


namespace Ligg.Infrastructure.Helpers
{

    internal static partial class ExcelHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //*convert
        internal static DataTable ConvertToDataTable(string filePath, string sheetName = null, bool hasColumn = true, short startRowIndex = 0, short startColumnIndex = 0, short rowQty = 0, short columnQty = 0)
        {
            var dt = ExcelToDataTable(filePath, sheetName, hasColumn, startRowIndex, startColumnIndex, rowQty, columnQty);
            return dt;
        }


    }
}
