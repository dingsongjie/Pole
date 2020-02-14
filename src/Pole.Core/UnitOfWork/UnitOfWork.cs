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

namespace Pole.Core.UnitOfWork
{
    class UnitOfWork : IUnitOfWork
    {
        private readonly IProducer producer;
        private readonly IEventTypeFinder eventTypeFinder;
        private readonly ISerializer serializer;
        private readonly IEventStorage eventStorage;
        private IBus bus;
        public UnitOfWork(IProducer producer, IEventTypeFinder eventTypeFinder, ISerializer serializer, IEventStorage eventStorage)
        {
            this.producer = producer;
            this.eventTypeFinder = eventTypeFinder;
            this.serializer = serializer;
            this.eventStorage = eventStorage;
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
                await eventStorage.ChangePublishStateAsync(@event, EventStatus.Published);
            });
        }

        public void Dispose()
        {
            bus = null;
        }

        public IUnitOfWork Enlist(IDbTransaction dbTransaction, IBus bus)
        {
            bus.Transaction = bus.ServiceProvider.GetService<IDbTransactionAdapter>();
            bus.Transaction.DbTransaction = dbTransaction;
            this.bus = bus;
            return this;
        }

        public Task Rollback(CancellationToken cancellationToken = default)
        {
            return bus.Transaction.RollbackAsync();
        }
    }
}
