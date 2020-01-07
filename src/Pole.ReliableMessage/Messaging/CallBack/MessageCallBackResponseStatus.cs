using Pole.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Messaging.CallBack
{
    public class MessageCallBackResponseStatus : Enumeration
    {
        public static MessageCallBackResponseStatus Finised = new MessageCallBackResponseStatus(3, "已完成");
        public static MessageCallBackResponseStatus Unfinished = new MessageCallBackResponseStatus(6, "未完成");
        public static MessageCallBackResponseStatus Unusual = new MessageCallBackResponseStatus(9, "异常");

        public MessageCallBackResponseStatus(int id, string name) : base(id, name)
        {
        }
    }
}
