using Ligg.Infrastructure.Extensions;
using NLog;
using System;


namespace Ligg.Infrastructure.Utilities.LogUtil
{
    public interface ILogHandler
    {

        void  Trace(object msg, Exception ex = null);

        void Debug(object msg, Exception ex = null);


        void Info(object msg, Exception ex = null);

        void Warn(object msg, Exception ex = null);


        void Error(object msg, Exception ex = null);
        void Error(Exception ex);

        void Fatal(object msg, Exception ex = null);
        void Fatal(Exception ex);

    }
}

