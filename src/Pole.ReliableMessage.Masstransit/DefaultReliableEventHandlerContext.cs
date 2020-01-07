using Pole.ReliableMessage.Abstraction;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Masstransit
{
    public class DefaultReliableEventHandlerContext<TEvent> : IReliableEventHandlerContext<TEvent>
        where TEvent : class
    {
        private readonly ConsumeContext<TEvent> _executeContext;
        public DefaultReliableEventHandlerContext(ConsumeContext<TEvent> executeContext)
        {
            _executeContext = executeContext;
            this.Event = executeContext.Message;
        }
        public TEvent Event { get; private set; }
    }
}
