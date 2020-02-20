using Pole.Core.EventBus;
using Pole.Core.EventBus.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pole.Core.Abstraction;
using Pole.Core.Serialization;
using Pole.Core.EventBus.Event;
using Pole.Core.EventBus.EventStorage;
using Microsoft.Extensions.Options;

namespace Pole.Core.UnitOfWork
{
    class UnitOfWork : IUnitOfWork
    {
        private readonly IProducer producer;
        private readonly IEventTypeFinder eventTypeFinder;
        private readonly ISerializer serializer;
        private readonly IEventStorage eventStorage;
        private readonly PoleOptions options;
        private IBus bus;
        public UnitOfWork(IProducer producer, IEventTypeFinder eventTypeFinder, ISerializer serializer, IEventStorage eventStorage, IOptions<PoleOptions> options)
        {
            this.producer = producer;
            this.eventTypeFinder = eventTypeFinder;
            this.serializer = serializer;
            this.eventStorage = eventStorage;
            this.options = options.Value;
        }

        public async Task CompeleteAsync(CancellationToken cancellationToken = default)
        {

            await bus.Transaction.CommitAsync();

            var bufferedEvents = bus.PrePublishEventBuffer.ToList();
            bufferedEvents.ForEach(async @event =>
            {
                var eventType = eventTypeFinder.FindType(@event.Name);
                var eventContentBytes = serializer.SerializeToUtf8Bytes(@event, eventType);
                var bytesTransport = new EventBytesTransport(@event.Name, @event.Id, eventContentBytes);
                var bytes = bytesTransport.GetBytes();
                await producer.Publish(bytes);
                @event.StatusName = nameof(EventStatus.Published);
                @event.ExpiresAt = DateTime.UtcNow.AddSeconds(options.PublishedEventsExpiredAfterSeconds);
            });
            await eventStorage.BulkChangePublishStateAsync(bufferedEvents);
        }

        public void Dispose()
        {
            bus = null;
        }

        public IUnitOfWork Enlist(IDbTransactionAdapter dbTransactionAdapter, IBus bus)
        {
            bus.Transaction = dbTransactionAdapter;
            this.bus = bus;
            return this;
        }

        public Task Rollback(CancellationToken cancellationToken = default)
        {
            return bus.Transaction.RollbackAsync();
        }
    }
}
