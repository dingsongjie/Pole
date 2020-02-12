using Pole.Core.Abstraction;
using Pole.Core.EventBus.Event;
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
        public Bus(IProducer producer, IEventTypeFinder eventTypeFinder, ISerializer serializer, ISnowflakeIdGenerator snowflakeIdGenerator)
        {
            this.producer = producer;
            this.eventTypeFinder = eventTypeFinder;
            this.serializer = serializer;
            this.snowflakeIdGenerator = snowflakeIdGenerator;
        }
        public async Task<bool> Publish(object @event, CancellationToken cancellationToken = default)
        {
            var eventType = @event.GetType();
            var eventTypeCode = eventTypeFinder.GetCode(eventType);
            var eventId = snowflakeIdGenerator.NextId();
            var bytesTransport = new EventBytesTransport(eventTypeCode, eventId, serializer.SerializeToUtf8Bytes(@event, eventType));
            var bytes = bytesTransport.GetBytes();
            await producer.Publish(bytes);
            return true;
        }
    }
}
