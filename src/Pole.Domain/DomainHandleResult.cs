using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Domain
{
    public class DomainHandleResult
    {
        public DomainHandleResult(int status, string message)
        {
            Status = status;
            Message = message;
        }
        public static DomainHandleResult SuccessResult = new DomainHandleResult(1, "处理成功");
        /// <summary>
        /// 1  Success 2  Faild ...
        /// </summary>
        public int Status { get;private set; }
        public string Message { get;private set; }
    }
}
