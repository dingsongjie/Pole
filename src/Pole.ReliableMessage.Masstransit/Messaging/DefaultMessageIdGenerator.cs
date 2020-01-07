using Pole.ReliableMessage.Abstraction;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Masstransit.Messaging
{
    class DefaultMessageIdGenerator : IMessageIdGenerator
    {
        public string Generate()
        {
            return NewId.Next().ToString("N").ToLowerInvariant();
        }
    }
}
