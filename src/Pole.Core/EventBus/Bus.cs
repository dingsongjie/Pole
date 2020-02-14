using Pole.Core.Abstraction;
using Pole.Core.EventBus.Event;
using Pole.Core.EventBus.EventStorage;
using Pole.Core.EventBus.Transaction;
using Pole.Core.Serialization;
using Pole.Core.Utils.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Core.EventBus
{
    class Bus : IBus
    {
        private readonly IProducer producer;
        private readonly IEventTypeFinder eventTypeFinder;
        private readonly ISerializer serializer;
        private readonly ISnowflakeIdGenerator snowflakeIdGenerator;
        private readonly IEventStorage eventStorage;
        public IDbTransactionAdapter Transaction { get; set; }

        public IServiceProvider ServiceProvider { get; }

        public Bus(IServiceProvider serviceProvider, IProducer producer, IEventTypeFinder eventTypeFinder, ISerializer serializer, ISnowflakeIdGenerator snowflakeIdGenerator, IEventStorage eventStorage)
        {
            ServiceProvider = serviceProvider;
            this.producer = producer;
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
            var eventContentBytes = serializer.SerializeToUtf8Bytes(@event, eventType);
            var eventContent = serializer.Serialize(@event, eventType);
            var bytesTransport = new EventBytesTransport(eventTypeCode, eventId, eventContentBytes);
            var bytes = bytesTransport.GetBytes();
            var eventEntity = new EventEntity
            {
                Added = DateTime.UtcNow,
                Content = eventContent,
                ExpiresAt = null,
                Id = eventId,
                Name = eventTypeCode,
                Retries = 0,
                StatusName = nameof(EventStatus.PrePublish)
            };
            if (Transaction?.DbTransaction == null)
            {
                var mediumMessage = await eventStorage.StoreMessage(eventEntity);
            }
            else
            {
                var mediumMessage = eventStorage.StoreMessage(eventEntity, Transaction.DbTransaction);

                if (Transaction.AutoCommit)
                {
                    await Transaction.CommitAsync();
                }
            }

            await producer.Publish(bytes);
            return true;
        }
    }
}
