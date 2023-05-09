
using System;

namespace Ligg.Infrastructure.Utilities.LogUtil
{
    public class LogHelper
    {

        public static LogFactory LogHandler;

        public static void Trace(object msg, Exception ex = null)
        {
            LogHandler.Handler.Trace(msg, ex);
        }

        public static void Debug(object msg, Exception ex = null)
        {
            //LogHandler.Handler.Trace(msg, ex);
        }

        public static void Info(object msg, Exception ex = null)
        {
            LogHandler.Handler.Info(msg, ex);
        }

        public static void Warn(object msg, Exception ex = null)
        {
            LogHandler.Handler.Warn(msg, ex);
        }

        public static void Error(object msg, Exception ex = null)
        {
            LogHandler.Handler.Error(msg, ex);
        }

        public static void Error(Exception ex)
        {
            LogHandler.Handler.Error(ex);
        }

        public static void Fatal(object msg, Exception ex = null)
        {
            LogHandler.Handler.Fatal(msg, ex);
        }

        public static void Fatal(Exception ex)
        {
            LogHandler.Handler.Fatal(ex);
        }


    }
}

