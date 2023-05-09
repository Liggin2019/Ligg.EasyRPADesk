using System;
using System.Data;
using Ligg.Infrastructure.Helpers;

namespace Ligg.Infrastructure.Utility.DataParser
{
    internal static class DataTableHelperEx
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //*convert
        internal static string ConvertToJsonEx(this DataTable dt, bool isEntitityFormat)
        {
            if (dt == null) return "";
            if (dt.Rows.Count == 0) return "";

            var json = "";
            if (isEntitityFormat)
            {
                dt = dt.ConvertEntityDataTable(true);
            }

            json = dt.ConvertToJson();

            if (isEntitityFormat)
            {
                json = json.Substring(1, json.Length - 1);
                json = json.Substring(0, json.Length - 1);
            }
            return json;
        }

        internal static DataTable ConvertEntityDataTable(this DataTable dt, bool V2H)
        {
            if (dt == null) throw new ArgumentException(_typeFullName + ".ConvertEntityDataTable Error: " + "DataTable can't be null!");
            var result = new DataTable();
            if (V2H) ////input dt header is FieldName&FieldValue vertical , to one row , no header
            {
                if (!dt.IsEntityFormat())
                    throw new ArgumentException(_typeFullName + ".ConvertEntityDataTable Error: " + "DataTable is not in EntityFormat!");

                DataRow dr = result.NewRow();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    result.Columns.Add(dt.Rows[i]["FieldName"].ToString());
                    dr[i] = dt.Rows[i]["FieldValue"].ToString();
                }
                result.Rows.Add(dr);
            }
            else//from input one row , no header , to header is FieldName&FieldValue vertical, many rows
            {
                result.Columns.Add("FieldName");
                result.Columns.Add("FieldValue");
                
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    DataRow dr = result.NewRow();
                    dr[0]= dt.Columns[i].ColumnName.ToString();
                    result.Rows.Add(dr);
                }

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    result.Rows[i][1] = dt.Rows[0][i].ToString();
                }
            }

            return result;
        }

        //*judge
        internal static bool IsEntityFormat(this DataTable dt)
        {
            if (dt == null) throw new ArgumentException(_typeFullName + ".IsEntityFormat Error: " + "DataTable can't be null!");
            if (dt.Columns.Count < 2 | dt.Rows.Count < 1) return false;
            if (dt.Columns[0].ToString() != "FieldName") return false;
            if (dt.Columns[1].ToString() != "FieldValue") return false;
            return true;
        }


    }
}
