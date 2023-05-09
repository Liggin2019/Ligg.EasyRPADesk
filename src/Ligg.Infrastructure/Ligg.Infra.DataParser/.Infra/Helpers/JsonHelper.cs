// using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
                var obj= Deserialize<T>(jsonStr, dateTimeFormat); 
                return obj;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".ConvertToGeneric Error: " + ex.Message);
            }
        }


        internal static string Serialize(object obj, string dateTimeFormat = "")
        {
            try
            {
                if (string.IsNullOrEmpty(dateTimeFormat))
                {
                    //to DataTime data, in thr form of 2022-05-08T11:47:25.4346386+08:00; it is local time, +08:00 means Time Area
                    return JsonConvert.SerializeObject(obj) ;
                }
                return JsonConvert.SerializeObject(obj, new IsoDateTimeConverter { DateTimeFormat = dateTimeFormat });
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".Serialize Error: " + ex.Message);
            }
        }


        private static T Deserialize<T>(string jsonStr, string dateTimeFormat = "")
        {
            try
            {
                if (string.IsNullOrEmpty(dateTimeFormat))
                {
                    return JsonConvert.DeserializeObject<T>(jsonStr);
                }

                return JsonConvert.DeserializeObject<T>(jsonStr, new IsoDateTimeConverter { DateTimeFormat = dateTimeFormat });
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".Deserialize Error: " + ex.Message);
            }
        }


    }
}
