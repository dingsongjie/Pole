using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Application.EventBus
{
    public interface IBus
    {
        Task Publish<TReliableEvent>(TReliableEvent @event, object callbackParemeter, CancellationToken cancellationToken = default) where TReliableEvent : class;
    }
}
