using System;
using System.Collections.Generic;
using System.Text;

namespace Ligg.Infrastructure.Extensions
{
    public static partial class ExceptionExtension
    {
        public static Exception GetOriginalException(this Exception ex)
        {
            if (ex.InnerException == null) return ex;

            return ex.InnerException.GetOriginalException();
        }


        public static string GetExceptionMessage(this Exception ex)
        {
            string message = string.Empty;
            if (ex != null)
            {
                message += ex.Message;
                message += Environment.NewLine;
                Exception originalException = ex.GetOriginalException();
                if (originalException != null)
                {
                    if (originalException.Message != ex.Message)
                    {
                        message += originalException.Message;
                        message += Environment.NewLine;
                    }
                }
                message += ex.StackTrace;
                message += Environment.NewLine;
            }
            return message;
        }

    }

}


