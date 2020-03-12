using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pole.Core;
using Pole.EventBus;
using Pole.EventBus.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PoleRabbitmqStartupConfigExtensions
    {
        private static ConcurrentDictionary<string, ConsumerRunner> ConsumerRunners = new ConcurrentDictionary<string, ConsumerRunner>();
        public static void AddRabbitMQ(
            this StartupConfig startupOption,
            Action<RabbitOptions> rabbitConfigAction,
            Func<IRabbitEventBusContainer, Task> eventBusConfig = default)
        {
            startupOption.Services.Configure<RabbitOptions>(config => rabbitConfigAction(config));
            startupOption.Services.AddSingleton<IRabbitMQClient, RabbitMQClient>();
            //startupOption.Services.AddHostedService<ConsumerManager>();
            startupOption.Services.AddSingleton<IRabbitEventBusContainer, EventBusContainer>();
            startupOption.Services.AddSingleton<IProducer, RabbitProducer>();
            startupOption.Services.AddSingleton(serviceProvider => serviceProvider.GetService<IRabbitEventBusContainer>() as IProducerInfoContainer);
            Startup.Register(async serviceProvider =>
            {
                var container = serviceProvider.GetService<IRabbitEventBusContainer>();
                var client = serviceProvider.GetService<IRabbitMQClient>();
                var rabbitOptions = serviceProvider.GetService<IOptions<RabbitOptions>>().Value;
                if (eventBusConfig != default)
                    await eventBusConfig(container);
                else
                    await container.AutoRegister();

                var consumers = container.GetConsumers();
                foreach (var consumer in consumers)
                {
                    if (consumer is RabbitConsumer value)
                    {
                        var queue = value.QueueInfo;
                        var key = queue.Queue;

                        var runner = new ConsumerRunner(client, serviceProvider, value, queue, rabbitOptions);
                        ConsumerRunners.TryAdd(key, runner);
                        await runner.Run();
                    }
                }
            });
        }
    }
}
