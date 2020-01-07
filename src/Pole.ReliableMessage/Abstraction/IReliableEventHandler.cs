using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IReliableEventHandler<TEvent> : IReliableEventHandler
        where TEvent : class
    {
        Task Handle(IReliableEventHandlerContext<TEvent> context);

    }
    public interface IReliableEventHandler
    {

    }
}
