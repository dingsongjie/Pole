using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IMessageCallBackRegister
    {
        Task Register(IEnumerable<Type> eventHandlerTypes);
    }
}
