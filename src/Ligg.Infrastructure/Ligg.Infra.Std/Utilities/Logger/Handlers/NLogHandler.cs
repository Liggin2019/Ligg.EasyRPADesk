using Ligg.Infrastructure.Extensions;
using NLog;
using System;



namespace Ligg.Infrastructure.Utilities.LogUtil
{
    public class NLogHandler : ILogHandler
    {

        private readonly Logger _log = LogManager.GetLogger(string.Empty);

        public void Trace(object msg, Exception ex = null)
        {
            if (ex == null)
            {
                _log.Trace(msg.ToString());
            }
            else
            {
                _log.Trace(msg + ex.GetExceptionMessage());
            }
        }

        public void Debug(object msg, Exception ex = null)
        {
            if (ex == null)
            {
                _log.Debug(msg.ToString());
            }
            else
            {
                _log.Debug(msg + ex.GetExceptionMessage());
            }
        }

        public void Info(object msg, Exception ex = null)
        {
            if (ex == null)
            {
                _log.Info(msg.ToString());
            }
            else
            {
                _log.Info(msg + ex.GetExceptionMessage());
            }
        }

        public void Warn(object msg, Exception ex = null)
        {
            if (ex == null)
            {
                _log.Warn(msg.ToString());
            }
            else
            {
                _log.Warn(msg + ex.GetExceptionMessage());
            }
        }

        public void Error(object msg, Exception ex = null)
        {
            if (ex == null)
            {
                _log.Error(msg.ToString());
            }
            else
            {
                _log.Error(msg + ex.GetExceptionMessage());
            }
        }

        public void Error(Exception ex)
        {
            if (ex != null)
            {
                _log.Error(ex.GetExceptionMessage());
            }
        }

        public void Fatal(object msg, Exception ex = null)
        {
            if (ex == null)
            {
                _log.Fatal(msg.ToString());
            }
            else
            {
                _log.Fatal(msg + ex.GetExceptionMessage());
            }
        }

        public void Fatal(Exception ex)
        {
            if (ex != null)
            {
                _log.Fatal(ex.GetExceptionMessage());
            }
        }


    }
}

