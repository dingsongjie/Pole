using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.Masstransit.Pipe;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Masstransit
{
    class MasstransitBasedMessageBus : IMessageBus
    {
        public MasstransitBasedMessageBus(MassTransit.IBus bus)
        {
            _bus = bus;
        }
        private readonly MassTransit.IBus _bus;
        public Task Publish(object @event, string reliableMessageId, CancellationToken cancellationToken = default)
        {
            var pipe = new AddReliableMessageIdPipe(reliableMessageId);
            return _bus.Publish(@event, pipe, cancellationToken);
        }
    }
}
