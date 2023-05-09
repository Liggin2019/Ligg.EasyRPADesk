using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.DataModels;
using System.IO;
using System.Xml;

namespace Ligg.Infrastructure.Helpers
{
    public static class DataTableHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //*convert
        public static string ConvertToJson(this DataTable dt)
        {
            if (dt == null) return "";
            if (dt.Rows.Count == 0) return "";

            return JsonHelper.Serialize(dt);
        }



        public static string ConvertToRichText(this DataTable dt, bool hasHead, string[] excludedfieldArray)
        {
            if (dt == null) return "";
            if (dt.Rows.Count == 0) return "";

            var headStr = "";
            var strBlder = new StringBuilder();

            if (dt.Rows.Count > 0)
            {
                var exclude = false;
                if (excludedfieldArray != null)
                {
                    if (excludedfieldArray.Count() != 0) exclude = true;

                }
                if (hasHead)
                {
                    var tm = 0;
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        var columnName = dt.Columns[j].ColumnName;
                        if (exclude)
                        {
                            if (!columnName.IsBeContainedInStringArray(excludedfieldArray, false))
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
                        else
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
                    strBlder.AppendLine(headStr);
                }


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var txt = "";
                    var ct = 0;
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        var columnName = dt.Columns[j].ColumnName;
                        if (exclude)
                        {
                            if (!columnName.IsBeContainedInStringArray(excludedfieldArray, false))
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
                        else
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
                    strBlder.AppendLine(txt);
                }
            }
            return strBlder.ToString();
        }

    }
}
