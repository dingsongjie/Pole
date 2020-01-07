using Pole.ReliableMessage.Messaging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IMessageBuffer
    {
        Task Flush();
        Task<bool> Add(Message message);
        Task<List<Message>> GetAll(Func<Message,bool> filter);
    }
}
