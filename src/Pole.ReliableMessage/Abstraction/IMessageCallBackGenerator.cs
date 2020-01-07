using Pole.ReliableMessage.Messaging.CallBack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IMessageCallBackInfoGenerator
    {
        MessageCallBackInfo Generate(Type eventHandlerType);
    }
}
