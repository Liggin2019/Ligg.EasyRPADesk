using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.RpaDesk.Parser.DataModels;
using Ligg.RpaDesk.Interface;
using System;

namespace Ligg.RpaDesk
{
    internal static class Bootstrapper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        internal static IStdServiceComponentAdapter Adapter = null;
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

}