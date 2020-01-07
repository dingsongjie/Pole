using Pole.ReliableMessage.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Messaging
{
    class DefaultMessageTypeIdGenerator : IMessageTypeIdGenerator
    {
        public string Generate(Type messageType)
        {
            return messageType.FullName;
        }
    }
}
