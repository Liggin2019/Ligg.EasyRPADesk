using Ligg.RpaDesk.Interface;
using Ligg.StandardServiceComponent;
using System;
using System.Reflection;
using System.Collections.Generic;
using Ligg.Infrastructure.Helpers;

namespace Ligg.StandardServices
{
    internal class AttendanceService : IStandardService
    {
        private static readonly string _typeName = MethodBase.GetCurrentMethod().ReflectedType.Name;
        private static readonly string _typeFullName = MethodBase.GetCurrentMethod().ReflectedType.FullName;

        private static string _outDirectory = CentralData.ApplicationOutputDir + "\\StdServices\\" + Adapter.ComponentName + "\\" + _typeName;
        private static string _libDirectory = CentralData.ApplicationLibDir;
        private string _errorLogPath = _outDirectory + "\\ErrLog-" + DateTime.Now.ToString("yyMMdd") + ".txt";
        public AttendanceService()
        {
            Initialize();
        }
        private void Initialize()
        {
        }
        public string Get(string function, string[] paramArr)
        {
            var exInfo = _typeFullName + ".Get." + function + " Error: ";
            var exInfoNoManed = _typeFullName + ".Get" + function + " Error, at " + DateTime.Now.ToString("yy-MM-dd-HH-mm-ss");
            if (function != "GetData" & function != "GetDetails")
            {
                if (!Adapter.NoManned) throw new ArgumentException(exInfo + " function is incorrect, function= " + function);
                //else FileHelper.SaveContentToTextFile(_errorLogPath, exInfoNoManed + "\nfunction is incorrect, function= " + function, true);
            }

            try
            {
                return Dispense(function, paramArr);
            }
            catch (Exception ex)
            {
                if (!Adapter.NoManned) throw new ArgumentException(exInfo + ex.Message);
                //else FileHelper.SaveContentToTextFile(_errorLogPath, exInfoNoManed + "\n" + ex.Message, true);
            }
            return null;
        }

        public void Do(string function, string[] paramArr)
        {

            throw new NotImplementedException();
        }

        private string Dispense(string function, string[] paramArr)
        {
            if (function == "GetData")
            {
                var time = DateTime.Now.AddMonths(-1);
                var lastMonth = time.ToString("yyyy-MM");
                var data = new List<Attendance>();
                var obj = new Attendance();
                obj.Id = 1; obj.Name = "Tom"; obj.LeaveTime = 2.1F; obj.LeaveEarlyTime = 8.1F; obj.BeLateTime = 6.3F; obj.AbsentTime = 2.2F; obj.OverDutyTime=3f; obj.WorkingTime = 150F; obj.Month = lastMonth;
                data.Add(obj);
                var obj1 = new Attendance();
                obj1.Id = 2; obj1.Name = "Jack"; obj1.LeaveTime = 5.1F; obj1.LeaveEarlyTime = 10.1F; obj1.BeLateTime = 2.4F; obj1.AbsentTime = 1F; obj1.OverDutyTime =2f; obj1.WorkingTime = 150F; obj1.Month = lastMonth;
                data.Add(obj1);
                var rst = GenericHelper.ConvertToJson<List<Attendance>>(data);
                return rst;

            }
            else if (function == "GetDetails")
            {

            }
            return null;
        }


    }


    internal class Attendance
    {
        public int Id;
        public string Name;
        public float LeaveTime;
        public float LeaveEarlyTime;
        public float BeLateTime;
        public float AbsentTime;
        public float OverDutyTime;
        public float WorkingTime;
        public string Month;
    }
}






