using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Domain.UnitOfWork
{
    public class UnitOfWorkResult
    {
        public static UnitOfWorkResult SuccessResult = new UnitOfWorkResult(1, "保存成功");
        public static UnitOfWorkResult FaildResult = new UnitOfWorkResult(1, "保存失败");
        public UnitOfWorkResult(int status, string message)
        {
            Status = status;
            Message = message;
        }

        /// <summary>
        /// 1  Success 2  Faild ...
        /// </summary>
        public int Status { get;private set; }
        public string Message { get;private set; }
    }
}
