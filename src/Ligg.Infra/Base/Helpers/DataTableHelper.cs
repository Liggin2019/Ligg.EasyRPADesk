using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.DataModel.Enums;

namespace Ligg.Infrastructure.Base.Helpers
{
    public static class DataTableHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static DataTable Shrink(DataTable dt, TernaryOption includeOrExlude, string[] fieldArry)
        {
            if (includeOrExlude == TernaryOption.None) return dt;
            if (dt.Rows.Count == 0) return dt;
            if (fieldArry.Length == 0) return dt;

            try
            {
                var validFieldNameArry = new string[fieldArry.Length];
                var i = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    if (includeOrExlude == TernaryOption.Forward)
                    {
                        if (col.ColumnName.IsBeContainedInStringArray(fieldArry))
                            validFieldNameArry[i] = col.ColumnName;
                    }
                    else
                    {
                        if (!col.ColumnName.IsBeContainedInStringArray(fieldArry))
                            validFieldNameArry[i] = col.ColumnName;
                    }
                    i++;

                }

                var result = new DataTable();
                DataView dv = new DataView(dt);
                result = dv.ToTable(true, validFieldNameArry);

                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".Shrink Error: " + ex.Message);
            }
        }
        public static IList<T> ConvertToList<T>(DataTable dt)
        {
            try
            {
                var result = new List<T>();
                //T t = (T)Activator.CreateInstance(typeof(T));
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    var t = System.Activator.CreateInstance(typeof(T));
                    PropertyInfo[] propertys = t.GetType().GetProperties();
                    foreach (PropertyInfo pi in propertys)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            // 属性与字段名称一致的进行赋值
                            if (pi.Name.Equals(dt.Columns[i].ColumnName))
                            {
                                // 数据库NULL值单独处理
                                if (dt.Rows[j][i] != DBNull.Value)
                                {
                                    var objVal = dt.Rows[j][i].ToString().ConvertToAnyType(pi.PropertyType, '`', '~');
                                    pi.SetValue(t, objVal, null);
                                }
                                else
                                    pi.SetValue(t, null, null);
                                break;
                            }
                        }
                    }
                    result.Add((T)t);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToList Error: " + ex.Message);
            }
        }

        public static string ConvertToJson(DataTable dt, bool isEntitityFormat)
        {
            try
            {
                var JsonString = new StringBuilder();
                if (dt.Rows.Count > 0)
                {
                    var rowNo = isEntitityFormat ? 1 : dt.Rows.Count;
                    if (!isEntitityFormat) JsonString.Append("[");
                    for (int i = 0; i < rowNo; i++)
                    {
                        JsonString.Append("{");
                        var stringList = new List<string>();
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            stringList.Add("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\"");
                        }
                        var str = "";
                        int c = 0;
                        foreach (var str1 in stringList)
                        {
                            if (c == 0) str = str1;
                            else
                                str = str + "," + str1;
                            c++;
                        }

                        JsonString.Append(str);

                        if (i == rowNo - 1)
                        {
                            JsonString.Append("}");
                        }
                        else
                        {
                            JsonString.Append("},");

                        }
                    }
                    if (!isEntitityFormat) JsonString.Append("]");
                }
                return JsonString.ToString();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToJson Error: " + ex.Message);
            }
        }
        public static string ConvertToInserDbScript(DataTable dt, bool append, string keyFieldName, string[] excludedFields)
        {

            string header = string.Empty;
            string script = string.Empty;
            header = "insert into [" + dt.TableName + "] (";
            try
            {
                foreach (DataColumn clm in dt.Columns)
                {
                    if (excludedFields != null)
                    {
                        if (excludedFields.Count() != 0)
                        {
                            if (excludedFields.Contains(clm.ColumnName.ToString()))
                            {
                                continue;
                            }

                        }
                    }

                    header += "[" + clm.ColumnName + "],";
                }

                header = header.Remove(header.Length - 1) + ") values (";

                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    string values = string.Empty;
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (excludedFields != null)
                        {
                            if (excludedFields.Count() != 0)
                            {
                                if (excludedFields.Contains(dt.Columns[j].ColumnName.ToString()))
                                {
                                    continue;
                                }
                            }
                        }


                        if (dt.Columns[j].ColumnName.ToString() == keyFieldName)
                        {
                            values += "" + "newid()" + ",";
                        }
                        else if (dt.Rows[i][j].ToString().ToLower() == "now")
                        {
                            values += "" + "GETDATE()" + ",";
                        }
                        else
                        {
                            values += "'" + dt.Rows[i][j].ToString() + "',";
                        }
                    }

                    script += header + values.Remove(values.Length - 1) + ")\n";
                }

                if (!append)
                {
                    script = "Delete [" + dt.TableName + "]" + "\n" + script;
                }
                return script;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToInserDbScript Error: " + ex.Message);
            }
        }

        public static string ConvertToAbpClassDefinitionCode(DataTable dt)
        {

            string header = string.Empty;
            string itemCodes = string.Empty;
            bool hasHead = false;
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string itemCode = string.Empty;
                    if (dt.Rows[i]["Name"].ToString() == "Id")
                    {
                        hasHead = true;
                        header = "public class " + dt.TableName + ": Entity<" + dt.Rows[i]["Type"] + ">\n{\n";
                        continue;
                    }
                    else
                    {
                        if (dt.Rows[i]["Required"].ToString().ToLower() == "y" | dt.Rows[i]["Required"].ToString().ToLower() == "yes")
                        {
                            itemCode = "[Required]\n";
                        }
                        if (dt.Rows[i]["Type"].ToString() == "String")
                        {
                            if (!dt.Rows[i]["StringLength"].ToString().ToLower().IsNullOrEmpty())
                            {
                                itemCode = itemCode + "[StringLength(" + dt.Rows[i]["StringLength"].ToString() + ")]\n";
                            }
                        }

                        itemCode = itemCode + "public " + dt.Rows[i]["Type"].ToString() + " " + dt.Rows[i]["Name"].ToString() + " { get; set; }\n";
                        itemCode = itemCode + "\n";
                        itemCodes = itemCodes + itemCode;
                    }
                }

                if (!hasHead) header = "public class " + dt.TableName + "\n{";

                return header + itemCodes + "}";
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToAbpClassDefinitionCode Error: " + ex.Message);
            }
        }

        public static string ConvertToRichText(DataTable dt, bool hasHead, string[] excludedfieldArray)
        {

            var headStr = "";
            var strBlder = new StringBuilder();
            try
            {
                if (dt.Rows.Count > 0)
                {
                    if (hasHead)
                    {
                        var tm = 0;
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            var columnName = dt.Columns[j].ColumnName;
                            if (excludedfieldArray != null & excludedfieldArray.Count() != 0)
                            {
                                if (!columnName.IsBeContainedInStringArray(excludedfieldArray))
                                {
                                    if (tm == 0)
                                    {
                                        headStr = columnName;
                                    }
                                    else
                                    {
                                        headStr = headStr + "\t" + columnName;
                                    }
                                    tm++;
                                }
                            }
                        }
                        strBlder.AppendLine(headStr);
                    }


                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var txt = "";
                        var ct = 0;
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            var columnName = dt.Columns[j].ColumnName;
                            if (excludedfieldArray != null & excludedfieldArray.Count() != 0)
                            {
                                if (!columnName.IsBeContainedInStringArray(excludedfieldArray))
                                {
                                    if (ct == 0)
                                    {
                                        txt = dt.Rows[i][j].ToString();
                                    }
                                    else
                                    {
                                        txt = txt + "\t" + dt.Rows[i][j];
                                    }
                                    ct++;
                                }
                            }
                        }
                        strBlder.AppendLine(txt);
                    }
                }
                return strBlder.ToString();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToRichText Error: " + ex.Message);
            }
        }
    }
}
