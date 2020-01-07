using Pole.ReliableMessage.Messaging.CallBack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
   public interface IMessageCallBackInfoStore
    {
        Task Add(MessageCallBackInfo messageCallBackInfo);
        Task<MessageCallBackInfo> Get(string messageTypeId);
    }
}
