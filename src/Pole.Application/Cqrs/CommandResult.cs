using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Application.Cqrs
{
    public class CommandResult
    {
        public CommandResult(int status,string message)
        {
            Status = status;
            Message = message;
        }
        public static CommandResult SuccessResult = new CommandResult(1, "操作成功");
        /// <summary>
        /// 1 Command Success 2 Command Faild ...
        /// </summary>
        public int Status { get;private set; } 
        public string Message { get;private set; } 
    }
}
