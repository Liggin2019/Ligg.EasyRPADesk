using System;

namespace Ligg.Infrastructure.DataModels
{

    public class TResult
    {
        public TResult(int flag = 0, string message = "", string description = "")
        {
            Flag = flag;
            Message = message;
            Description = description;
        }
        public int Flag { get; set; }
        public ServiceResultCode Code { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }

    }

    public class TResult<T> : TResult
    {
        public TResult()
        {
        }
        public TResult(int flag, T data, string message="", string description="", int total=0)
        {
            Data = data;
            Flag = flag;
            Message = message;
            Description = description;
            Total = total;
        }


        public int Total { get; set; }

        public T Data { get; set; }



    }

    public enum ServiceResultCode
    {
        Succeed = 0,
        Failed = 1,
    }
}
