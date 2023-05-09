using Ligg.RpaDesk.Interface;
using Ligg.StandardServices;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Ligg.StandardServiceComponent
{
    public class Adapter : IStdServiceComponentAdapter
    {
        private static readonly string _typeFullName = MethodBase.GetCurrentMethod().ReflectedType.FullName;
        public Adapter()
        {
            Initialize(null);
        }
        internal static string ComponentName = "";
        internal static bool NoManned = false;

        public string Dispatch(string service, string method, string function, string[] paramArr)
        {

            Type type = null;
            if (service == "AttendanceService") type = typeof(AttendanceService);
            else throw new ArgumentException(_typeFullName + "." + ComponentName + ".Dispatch Error: Ligg.StandardService does not exist, service= " + service);
            var obj = Activator.CreateInstance(type);

            //if (service != "AttendanceService")
            //{
            //    throw new ArgumentException(_typeFullName + "." + ComponentName + ".Dispatch Error: Ligg.StandardService does not exist, service= " + service);
            //}
            //Type type = Type.GetType("Ligg.StandardServices." + service, false);
            //var obj = Activator.CreateInstance(type);

            IStandardService stdservice = obj as IStandardService;

            if (method == "Get")
            {
                return stdservice.Get(function, paramArr);
            }
            else if (method == "Do")
            {
                stdservice.Do(function, paramArr);
            }
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
        public string ResolveConstants(string text)
        {
            var toBeRplStr = "%OnlyLastMonthCanBeApproved%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = "Only last month can be Approved";
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
                return text;
            }
            return text;

        }
        public UniversalResult Validate(string input, string rule)
        {
            if(rule=="LastMonth")
            {
                var time = DateTime.Now.AddMonths(-1);
                var time1 = new DateTime(time.Year,time.Month,1).AddDays(-1);
                var time2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var inputTime = DateTime.MinValue;
                DateTime.TryParse(input,out inputTime);
                if (inputTime > time1 & inputTime < time2)
                    return new UniversalResult(true,"");
                else return new UniversalResult(false, input+ " is not LastMonth");
            }
            throw new NotImplementedException();
        }


    }
}
