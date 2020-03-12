using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pole.EventBus.RabbitMQ
{
    public class RabbitConsumer : Consumer
    {
        public RabbitConsumer(
            Func<byte[], Task> eventHandlers,
            Func<List<byte[]>, Task> batchEventHandlers) : base(new List<Func<byte[], Task>> { eventHandlers }, new List<Func<List<byte[]>, Task>> { batchEventHandlers })
        {
        }
        public RabbitEventBus EventBus { get; set; }
        public QueueInfo QueueInfo { get; set; }
        public ConsumerOptions Config { get; set; }
    }
}
