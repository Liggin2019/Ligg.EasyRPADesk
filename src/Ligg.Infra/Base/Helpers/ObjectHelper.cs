using System;
using System.Data;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using Newtonsoft.Json;


namespace Ligg.Infrastructure.Base.Helpers
{
    public static class ObjectHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //#convert
        //## AnyType
        static internal object ConvertToAnyType(object value, Type type)
        {
            object returnValue;
            if ((value == null) || type.IsInstanceOfType(value))
            {
                return value;
            }
            var str = value as string;
            if ((str != null) && (str.Length == 0))
            {
                return null;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(type);
            bool flag = converter.CanConvertFrom(value.GetType());
            if (!flag)
            {
                converter = TypeDescriptor.GetConverter(value.GetType());
            }
            if (!flag && !converter.CanConvertTo(type))
            {
                throw new InvalidOperationException("Can't Convert Type：" + value.ToString() + "==>" + type);
            }
            try
            {
                returnValue = flag ? converter.ConvertFrom(null, null, value) : converter.ConvertTo(null, null, value, type);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Convert Type：" + value.ToString() + "==>" + type, e);
            }
            return returnValue;
        }

        //##json
        public static string ConvertToJson(this object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToJson Error: " + ex.Message);
            }
        }


        //##ObjectArray to datatable
        public static DataTable ConvertObjectArrayToDataTable(object[] objArry)
        {
            try
            {
                if (objArry == null)
                {
                    return null;
                }
                else
                {
                    Type t = objArry[0].GetType();
                    var dt = new DataTable();
                    PropertyInfo[] properties = t.GetProperties();
                    foreach (PropertyInfo pro in properties)
                    {
                        dt.Columns.Add(new DataColumn(pro.Name, pro.PropertyType));
                    }

                    DataColumnCollection col = dt.Columns;
                    foreach (object obj in objArry)
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < col.Count; i++)
                        {
                            dr[i] = t.InvokeMember(col[i].ColumnName, BindingFlags.GetProperty, null, obj, null);//error
                        }
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertObjectArrayToDataTable Error: " + ex.Message);
            }
        }



        //##dataset
        public static DataSet ConvertObjectArrayToDataSet(object[] objArry)
        {
            try
            {
                if (objArry == null)
                {
                    return null;
                }
                else
                {
                    Type t = objArry[0].GetType();
                    var ds = new DataSet(); ;
                    var dt = new DataTable();
                    dt = ObjectHelper.ConvertObjectArrayToDataTable(objArry);
                    ds.Tables.Add(dt);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertObjectArrayToDataSet Error: " + ex.Message);
            }
        }

        //#judge
        public static bool IsType(Type type, string typeName)
        {
            try
            {
                if (type.ToString() == typeName) return true;
                if (type.ToString() == "System.Object") return false;
                return IsType(type.BaseType, typeName);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".IsType Error: " + ex.Message);
            }
        }


    }
}
