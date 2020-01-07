using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Domain.UnitOfWork
{
    public class CompleteResult
    {
        public static CompleteResult SuccessResult = new CompleteResult(1, "保存成功");
        public CompleteResult(int status, string message)
        {
            Status = status;
            Message = message;
        }
        public CompleteResult(DomainHandleResult domainHandleResult) : this(domainHandleResult.Status, domainHandleResult.Message) { }

        /// <summary>
        /// 1  Success 2  Faild ...
        /// </summary>
        public int Status { get;private set; }
        public string Message { get;private set; }
    }
}
