using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
// using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace Ligg.Infrastructure.Base.Helpers
{
    public static class JsonHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //#Convert
        public static T ConvertToGeneric<T>(string jsonStr)
        {
            try
            {
                return Deserialize<T>(jsonStr);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToGeneric Error: " + ex.Message);
            }
        }

        public static DataTable ConvertToDataTable(string jsonStr)
        {
            DataTable dataTable = new DataTable();
            DataTable result;
            var arrayList = new ArrayList();
            try
            {
                arrayList = Deserialize<ArrayList>(jsonStr);
                //* following can not  be used in net standard project, but can used in net fx projects, by using using System.Web.Script.Serialization
                //JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                //javaScriptSerializer.MaxJsonLength = Int32.MaxValue;
                //ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(jsonStr);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count<string>() == 0)
                        {
                            result = dataTable;
                            return result;
                        }

                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                dataTable.Columns.Add(current, dictionary[current].GetType());
                            }
                        }

                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            dataRow[current] = dictionary[current];
                        }

                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToDataTable Error: " + ex.Message);
            }
            result = dataTable;
            return result;
        }

        //it is also OK
        private static DataTable ConvertToDataTable1(string jsonStr)
        {
            try
            {
                DataTable dt = null;
                jsonStr = jsonStr.Replace(",\"", "*\"").Replace("\":", "\"#").ToString();

                //取出表名   
                var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
                string strName = rg.Match(jsonStr).Value;

                //去除表名   
                jsonStr = jsonStr.Substring(jsonStr.IndexOf("[") + 1);
                jsonStr = jsonStr.Substring(0, jsonStr.IndexOf("]"));

                //获取数据   
                rg = new Regex(@"(?<={)[^}]+(?=})");
                MatchCollection mc = rg.Matches(jsonStr);
                for (int i = 0; i < mc.Count; i++)
                {
                    string strRow = mc[i].Value;
                    string[] strRows = strRow.Split('*');

                    //创建表   
                    if (dt == null)
                    {
                        dt = new DataTable();
                        dt.TableName = strName;
                        foreach (string str in strRows)
                        {
                            var dc = new DataColumn();
                            string[] strCell = str.Split('#');
                            if (strCell[0].Substring(0, 1) == "\"")
                            {
                                int a = strCell[0].Length;
                                dc.ColumnName = strCell[0].Substring(1, a - 2);
                            }
                            else
                            {
                                dc.ColumnName = strCell[0];
                            }
                            dt.Columns.Add(dc);
                        }
                        dt.AcceptChanges();
                    }

                    //增加内容   
                    DataRow dr = dt.NewRow();
                    for (int r = 0; r < strRows.Length; r++)
                    {
                        dr[r] = strRows[r].Split('#')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                    }
                    dt.Rows.Add(dr);
                    dt.AcceptChanges();
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToDataTable Error: " + ex.Message);
            }
        }

        //#Serialize
        private static string Serialize(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".Serialize Error: " + ex.Message);
            }
        }

        private static string SerializeWithCamelCasePropertyName(object obj)
        {
            try
            {
                var setting = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                };
                return JsonConvert.SerializeObject(obj, Formatting.None, setting);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".SerializeWithCamelCasePropertyName Error: " + ex.Message);
            }
        }

        private static string SerializeByConverter(object obj, params JsonConverter[] converters)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, converters);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".SerializeByConverter Error: " + ex.Message);
            }
        }



        //#Deserialize
        private static T Deserialize<T>(string jsonStr)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonStr);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".Deserialize Error: " + ex.Message);
            }
        }

        private static T DeserializeByConverter<T>(string jsonStr, params JsonConverter[] converter)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonStr, converter);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".DeserializeByConverter Error: " + ex.Message);
            }
        }

        private static T DeserializeBySetting<T>(string jsonStr, JsonSerializerSettings settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonStr, settings);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".DeserializeBySetting Error: " + ex.Message);
            }
        }


    }
}
