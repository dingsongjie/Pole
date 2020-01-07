using Pole.ReliableMessage.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Messaging
{
    public class MessageStatus : Enumeration
    {
        public static MessageStatus Pending = new MessageStatus(3,"待发送");
        public static MessageStatus Pushed = new MessageStatus(6,"已发送");
        public static MessageStatus Canced = new MessageStatus(9,"已取消");
        public static MessageStatus Handed = new MessageStatus(12, "已处理");

        public MessageStatus(int id,string name ):base(id,name)
        {

        }     
    }
}
