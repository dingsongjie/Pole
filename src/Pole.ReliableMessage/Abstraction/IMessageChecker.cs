using Pole.ReliableMessage.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IMessageChecker
    {
       Task<MessageCheckerResult> GetResult(Message message);
      
    }
}
