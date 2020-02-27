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
using Pole.Core.Serialization;
using Pole.Core.EventBus.Event;
using Pole.Core.EventBus.EventStorage;
using Microsoft.Extensions.Options;
using Pole.Core.Utils.Abstraction;
using Pole.Core.Exceptions;

namespace Pole.Core.UnitOfWork
{
    class UnitOfWork : IUnitOfWork
    {
        private readonly IProducerInfoContainer producerContainer;
        private readonly IEventTypeFinder eventTypeFinder;
        private readonly ISerializer serializer;
        private IBus bus;
        private IEventBuffer eventBuffer;
        public IServiceProvider ServiceProvider { get; }
        public UnitOfWork(IProducerInfoContainer producerContainer, IEventTypeFinder eventTypeFinder,
            ISerializer serializer, IEventBuffer eventBuffer, IServiceProvider serviceProvider)
        {
            this.producerContainer = producerContainer;
            this.eventTypeFinder = eventTypeFinder;
            this.serializer = serializer;
            this.eventBuffer = eventBuffer;
            this.ServiceProvider = serviceProvider;
        }

        public async Task CompeleteAsync(CancellationToken cancellationToken = default)
        {

            await bus.Transaction.CommitAsync();

            var bufferedEvents = bus.PrePublishEventBuffer.ToList();
            bufferedEvents.ForEach(async @event =>
            {
                var eventType = eventTypeFinder.FindType(@event.Name);
                var eventContentBytes = Encoding.UTF8.GetBytes(@event.Content);
                var bytesTransport = new EventBytesTransport(@event.Name, @event.Id, eventContentBytes);
                var bytes = bytesTransport.GetBytes();
                var result = await eventBuffer.AddAndRun(@event);
                if (!result)
                {
                    throw new AddEventToEventBufferException();
                }
            });
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
