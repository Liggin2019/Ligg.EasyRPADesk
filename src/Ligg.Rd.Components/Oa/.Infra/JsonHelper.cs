// using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Data;


namespace Ligg.Infrastructure.Helpers
{
    internal static class JsonHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //*Convert

        internal static T ConvertToGeneric<T>(string jsonStr, string dateTimeFormat = "")
        {
            try
            {
                return Deserialize<T>(jsonStr, dateTimeFormat);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".ConvertToGeneric Error: " + ex.Message);
            }
        }



        //*private
        //*serialize
        internal static string Serialize(object obj, string dateTimeFormat = "")
        {
            try
            {
                if (string.IsNullOrEmpty(dateTimeFormat))
                {
                    //to DataTime data, in thr form of 2022-05-08T11:47:25.4346386+08:00; it is local time, +08:00 means Time Area
                    return JsonConvert.SerializeObject(obj);
                }
                return JsonConvert.SerializeObject(obj, new IsoDateTimeConverter { DateTimeFormat = dateTimeFormat });
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".Serialize Error: " + ex.Message);
            }
        }


        //*deserialize
        private static T Deserialize<T>(string jsonStr, string dateTimeFormat = "")
        {
            try
            {
                if (string.IsNullOrEmpty(dateTimeFormat))
                {
                    //if no new IsoDateTimeConverter
                    //if the DateTime data is like 2022-05-08T11:47:25.4346386+08:00; ok
                    //if the DateTime data is like 2022-01; ok
                    //if the DateTime data is like 2022-01-01; ok
                    //if the DateTime data is like 2022-05-08T11:47; ok
                    //if the DateTime data is like 2022-05-08T11:47:25; ok
                    //if the DateTime data is like 2022; will popup error
                    //if the DateTime data is like 2022-01-01 22:22:22, will popup error
                    return JsonConvert.DeserializeObject<T>(jsonStr);
                }

                //* if DateTime data is like 2020-01-01, dateTimeFormat is yyyy-MM-dd HH:mm:ss; popup error
                return JsonConvert.DeserializeObject<T>(jsonStr, new IsoDateTimeConverter { DateTimeFormat = dateTimeFormat });
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".Deserialize Error: " + ex.Message);
            }
        }

    }
}



