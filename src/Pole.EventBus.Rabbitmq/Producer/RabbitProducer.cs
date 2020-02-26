using Microsoft.Extensions.Options;
using Pole.Core;
using Pole.Core.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pole.EventBus.RabbitMQ
{
    public class RabbitProducer : IProducer
    {
        readonly IRabbitMQClient rabbitMQClient;
        readonly RabbitOptions rabbitOptions;
        public RabbitProducer(
            IRabbitMQClient rabbitMQClient,
            IOptions<RabbitOptions> rabbitOptions)
        {
            this.rabbitMQClient = rabbitMQClient;
            this.rabbitOptions = rabbitOptions.Value;
        }

        public ValueTask BulkPublish(IEnumerable<(string, byte[])> events)
        {
            using (var channel = rabbitMQClient.PullChannel())
            {
                events.ToList().ForEach(@event =>
                {
                    channel.Publish(@event.Item2, @event.Item1, string.Empty, true);
                });

                channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(rabbitOptions.ProducerConfirmWaitTimeoutSeconds));
            }
            return Consts.ValueTaskDone;
        }

        public ValueTask Publish(string targetName, byte[] bytes)
        {
            using (var channel = rabbitMQClient.PullChannel())
            {
                channel.Publish(bytes, targetName, string.Empty, true);
                channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(rabbitOptions.ProducerConfirmWaitTimeoutSeconds));
            }
            return Consts.ValueTaskDone;
        }
    }
}
