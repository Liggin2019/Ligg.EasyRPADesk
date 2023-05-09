using System;
using Ligg.RpaDesk.Interface;
using Ligg.StandardServices;

namespace Ligg.StandardServiceComponent
{
    public class Adapter : IStdServiceComponentAdapter
    {
        public static string BaseDirectory;
        public static string ComponentName;

        public string Dispatch(string service, string method, string function, string[] paramArr)
        {


            Type type = Type.GetType("Ligg.RpaDesk.StandardService." + service, false);
            if (type == null)
            {
                throw new ArgumentException("Service does not exist, typeName=" + "Ligg.RpaDesk.StandardService." + service);
            }
            //Type type = null;
            //if (service == "AttendanceService")
            //    type = typeof(AttendanceService);
            //else throw new ArgumentException("Ligg.RpaDesk.StandardService does not exist, typeName= " + service);

            var obj = Activator.CreateInstance(type);
            IStandardService stdservice = obj as IStandardService;

            if (method == "Get")
            {
                return stdservice.Get(function, paramArr);
            }
            else if (method == "Do")
            {
                stdservice.Do(function, paramArr);
            }
            //else if (method == "Judge")
            //{
            //    return stdservice.Judge(function, paramArr) ? "true" : "false";
            //}
            //else if (method == "Validate")
            //{
            //    var rst = stdservice.Validate(function, paramArr);
            //    //return JsonConvert.SerializeObject(rst, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            //    return string.Empty;
            //}
            //else if (method == "Score")
            //{
            //    var rst = stdservice.Score(function, paramArr);
            //    //return JsonConvert.SerializeObject(rst, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            //    return string.Empty;
            //}
            else
            {
                stdservice.Do(function, paramArr);
            }
            return null;

        }
        public void Initialize(string[] paramArr)
        {
            if (paramArr == null) ComponentName = GetType().Name;
        }
        public string ResolveConstants(string input)
        {
            throw new NotImplementedException();
        }
        public UniversalResult Validate(string input, string rule)
        {
            throw new NotImplementedException();
        }

       
    }
}
