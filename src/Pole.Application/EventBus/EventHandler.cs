using Pole.ReliableMessage.Masstransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Application.EventBus
{
    public abstract class IntegrationEventHandler<TEvent> : ReliableEventHandler<TEvent> where TEvent : class
    {
        public IntegrationEventHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
