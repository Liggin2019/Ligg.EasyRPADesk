using Ligg.Infrastructure.Extensions;

using NLog;
using System;


namespace Ligg.Infrastructure.Utilities.LogUtil
{
    public class LogFactory
    {
        public ILogHandler Handler;
        public LogFactory(string handler)
        {

            if (handler.ToLower() == "NLog".ToLower())
            {
                Handler = new NLogHandler();
            }
            else if (handler.ToLower() == "Log4Net".ToLower())
            {
                throw new NotImplementedException();
            }
            else
            {
                Handler = new NLogHandler();
            }
            

        }
    }
}

