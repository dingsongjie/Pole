using Pole.Core;
using Pole.Core.EventBus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pole.EventBus.RabbitMQ
{
    public class RabbitProducer : IProducer
    {
        readonly RabbitEventBus publisher;
        readonly IRabbitMQClient rabbitMQClient;
        readonly RabbitOptions rabbitOptions;
        public RabbitProducer(
            IRabbitMQClient rabbitMQClient,
            RabbitEventBus publisher,
            RabbitOptions rabbitOptions)
        {
            this.publisher = publisher;
            this.rabbitMQClient = rabbitMQClient;
            this.rabbitOptions = rabbitOptions;
        }
        public ValueTask Publish(byte[] bytes)
        {
            using var channel = rabbitMQClient.PullChannel();
            channel.Publish(bytes, $"{rabbitOptions.Prefix}{publisher.Exchange}", string.Empty, publisher.Persistent);
            return Consts.ValueTaskDone;
        }
    }
}
