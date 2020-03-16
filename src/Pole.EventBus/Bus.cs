
using Pole.EventBus.Event;
using Pole.EventBus.EventStorage;
using Pole.EventBus.Transaction;
using Pole.Core.Serialization;
using Pole.Core.Utils.Abstraction;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pole.EventBus.Abstraction;

namespace Pole.EventBus
{
    class Bus : IBus
    {
        private readonly IEventTypeFinder eventTypeFinder;
        private readonly ISerializer serializer;
        private readonly ISnowflakeIdGenerator snowflakeIdGenerator;
        private readonly IEventStorage eventStorage;
        public IDbTransactionAdapter Transaction { get; set; }

        public IServiceProvider ServiceProvider { get; }
        public BlockingCollection<EventEntity> PrePublishEventBuffer { get; } = new BlockingCollection<EventEntity>(new ConcurrentQueue<EventEntity>());

        public Bus(IServiceProvider serviceProvider, IEventTypeFinder eventTypeFinder, ISerializer serializer, ISnowflakeIdGenerator snowflakeIdGenerator, IEventStorage eventStorage)
        {
            ServiceProvider = serviceProvider;
            this.eventTypeFinder = eventTypeFinder;
            this.serializer = serializer;
            this.snowflakeIdGenerator = snowflakeIdGenerator;
            this.eventStorage = eventStorage;
        }
        public async Task<bool> Publish(object @event, CancellationToken cancellationToken = default)
        {
            var eventType = @event.GetType();
            var eventTypeCode = eventTypeFinder.GetCode(eventType);
            var eventId = snowflakeIdGenerator.NextId();
            var eventContent = serializer.Serialize(@event, eventType);
            var eventEntity = new EventEntity
            {
                Added = DateTime.UtcNow,
                Content = eventContent,
                ExpiresAt = null,
                Id = eventId,
                Name = eventTypeCode,
                Retries = 0,
                StatusName = nameof(EventStatus.Pending)
            };
            if (Transaction?.DbTransaction == null)
            {
                await eventStorage.StoreMessage(eventEntity);
            }
            else
            {
                await eventStorage.StoreMessage(eventEntity, Transaction.DbTransaction);

            }
            PrePublishEventBuffer.Add(eventEntity);

            return true;
        }
    }
}
