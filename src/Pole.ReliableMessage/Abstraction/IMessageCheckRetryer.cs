using Pole.ReliableMessage.Storage.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IMessageCheckRetryer
    {
        Task Execute(IEnumerable<Message> messages, DateTime dateTime);
    }
}
