using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IReliableBus
    {
        Task<string> PrePublish<TReliableEvent>(TReliableEvent @event, object callbackParemeter, CancellationToken cancellationToken = default) where TReliableEvent : class;
        Task<string> PrePublish(object @event, Type eventType, object callbackParemeter, CancellationToken cancellationToken = default);
        Task<bool> Publish(object @event, string prePublishMessageId, CancellationToken cancellationToken = default);
        Task<bool> Cancel(string prePublishMessageId, CancellationToken cancellationToken = default);
    }
}
