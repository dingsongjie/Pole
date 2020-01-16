using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Application.EventBus
{
    public class ReliableEventBus : IEventBus
    {
        private readonly IReliableMessageScopedBuffer _reliableMessageScopedBuffer;

        public ReliableEventBus(IReliableMessageScopedBuffer reliableMessageScopedBuffer)
        {
            _reliableMessageScopedBuffer = reliableMessageScopedBuffer;
        }

        public Task Publish<TReliableEvent>(TReliableEvent @event, object callbackParemeter, CancellationToken cancellationToken = default) where TReliableEvent : class
        {
            _reliableMessageScopedBuffer.Add(new EventEntry(@event, callbackParemeter,typeof(TReliableEvent)));
            return Task.FromResult(1);
        }
    }
}
