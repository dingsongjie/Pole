using Pole.EventBus.EventStorage;
using Pole.EventBus.Transaction;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.EventBus.Abstraction
{
    public interface IBus
    {
        IServiceProvider ServiceProvider { get; }
        IDbTransactionAdapter Transaction { get; set; }
        BlockingCollection<EventEntity> PrePublishEventBuffer { get; }
        Task<bool> Publish(object @event, CancellationToken cancellationToken = default);
    }
}
