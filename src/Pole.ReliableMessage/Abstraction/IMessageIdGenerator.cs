using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IMessageIdGenerator
    {
        string Generate();
    }
}
