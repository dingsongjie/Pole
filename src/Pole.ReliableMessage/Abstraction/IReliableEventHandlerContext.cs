using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
   public interface IReliableEventHandlerContext<TEvent> where TEvent : class
    {
        TEvent Event { get; }
        //Task Publish(object @event);
    }
}
