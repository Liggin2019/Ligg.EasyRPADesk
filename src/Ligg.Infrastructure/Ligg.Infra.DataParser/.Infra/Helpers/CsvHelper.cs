using Ligg.Infrastructure.Extensions;
using System.Data;

namespace Ligg.Infrastructure.Helpers
{
    internal static class CsvHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        internal static DataTable ConvertToDataTable(string csvText, bool hasColumn = true)
        {
            var ctt = csvText;
            if (ctt.IsNullOrEmpty()) return null;
            var dt = new DataTable();
            var rows = ctt.SplitByString("\r\n", false, true);
            var rowIndex = 0;
            foreach (var rowText in rows)
            {
                if (rowText.IsNullOrEmpty()) continue;
                var rowTextToArray = ConvertRowTextToArrayByExcelFormat(rowText);

                if (hasColumn)
                {
                    if (rowIndex == 0)
                    {
                        for (var colIndex = 0; colIndex < rowTextToArray.Length; colIndex++)
                        {
                            var colText = rowTextToArray[colIndex];
                            colText = ConvertColumnTextToExcelFormat(colText);
                            dt.Columns.Add(new DataColumn(colText));
                        }
                    }
                    else
                    {
                        var dr = dt.NewRow();
                        for (var colIndex = 0; colIndex < rowTextToArray.Length; colIndex++)
                        {
                            var colText = rowTextToArray[colIndex];
                            colText = ConvertColumnTextToExcelFormat(colText);
                            dr[colIndex] = colText;

                        }
                        dt.Rows.Add(dr);

                    }
                }
                else
                {
                    if (rowIndex == 0)
                    {
                        for (var colIndex = 0; colIndex < rowTextToArray.Length; colIndex++)
                        {
                            dt.Columns.Add(new DataColumn("Column" + colIndex.ToString()));
                        }

                        var dr = dt.NewRow();
                        for (var colIndex = 0; colIndex < rowTextToArray.Length; colIndex++)
                        {
                            var colText = rowTextToArray[colIndex];
                            colText = ConvertColumnTextToExcelFormat(colText);

                            dr[colIndex] = colText;
                        }
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        var dr = dt.NewRow();
                        for (var colIndex = 0; colIndex < rowTextToArray.Length; colIndex++)
                        {
                            var colText = rowTextToArray[colIndex];
                            colText = ConvertColumnTextToExcelFormat(colText);
                            dr[colIndex] = colText;
                        }
                        dt.Rows.Add(dr);
                    }
                }
                rowIndex++;
            }
            return dt;
        }

        internal static string[] ConvertRowTextToArrayByExcelFormat(string rowText)
        {
            rowText = rowText.Replace("\"\"", "QQUUOOTTEESS");
            var tmpArr = rowText.Split('"');
            for (var i = 0; i < tmpArr.Length; i++)
            {
                if (i % 2 == 1)
                {
                    if (tmpArr[i].Contains(","))
                    {
                        tmpArr[i] = tmpArr[i].Replace(",", "CCOOMMAA");
                    }
                }
            }

            var tmpStr1 = tmpArr.Unwrap();
            var tmpStrArr1 = tmpStr1.Split(',');
            return tmpStrArr1;
        }
        internal static string ConvertColumnTextToExcelFormat(string colText)
        {
            if (colText.Contains("CCOOMMAA"))
            {
                colText = colText.Replace("CCOOMMAA", ",");
            }
            if (colText.Contains("QQUUOOTTEESS"))
            {
                colText = colText.Replace("QQUUOOTTEESS", "\"");
            }
            return colText;
        }

        internal static string ConvertColumnTextToCsvFormat(string text)
        {
            var delimiter = "\"";
            var delimiter1 = ",";
            var delimiter2 = "\\n";
            if (text.Contains(delimiter))
            {
                text = text.Replace(delimiter, "\"\"");
            }

            if (text.Contains(delimiter) | text.Contains(delimiter1) | text.Contains(delimiter2)) text = "\"" + text + "\"";
            return text;
        }




    }
}
