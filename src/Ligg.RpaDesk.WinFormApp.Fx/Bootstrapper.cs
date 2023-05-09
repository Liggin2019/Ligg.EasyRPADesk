using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
//using Ligg.Infrastructure.Utilities.Ioc;
//using Ligg.RpaDesk.DataModels;
using Ligg.RpaDesk.Parser.DataModels;
using Ligg.RpaDesk.Interface;
using Ligg.RpaDesk.Plugin;
using Ligg.RpaDesk.Resources;
using Ligg.RpaDesk.WinForm.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Ligg.RpaDesk
{
    internal static class Bootstrapper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        internal static List<BootstrapperTask> _bootStrapperTasks = new List<BootstrapperTask>();
        internal static IStdServiceComponentAdapter Adapter = null;
        internal static HttpClient HttpClient = null;
        internal static void Initialize(ApplicationSetting applicationSetting, string appCode)
        {
            try
            {
                var httpClientBaseUrlLstr = applicationSetting.ApplicationDataDir + "\\HttpClientBaseUrl.lstr";
                var httpClientBaseUrl = FileHelper.GetContentFromTextFile(httpClientBaseUrlLstr);
                if (!httpClientBaseUrl.IsNullOrEmpty())
                {
                    HttpClientHelper.Initialize(httpClientBaseUrl);
                }

                ComponentHelper.Initialize(applicationSetting, appCode);
            }
            catch (Exception ex)
            {
                var msg = ".ComponentHelper.Initialize error: " + ex.Message;
                throw new ArgumentException("\n>> " + _typeFullName + msg);
            }

        }


    }

    public class BootstrapperTask
    {
        public IBootTask Task;
        public string Code;
        public string DllPath;
        public string ClassFullName;
        public BootstrapperTask(string code, string dllPath, string classFullName, IBootTask task)
        {
            Code = code;
            DllPath = dllPath;
            ClassFullName = classFullName;
            Task = task;
        }
    }
}