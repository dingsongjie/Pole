using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IMessageBus
    {
        Task Publish(object @event,string reliableMessageId, CancellationToken cancellationToken = default);
    }
}
