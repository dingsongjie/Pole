using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pole.Core;
using Pole.Core.EventBus;
using Pole.EventBus.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PoleRabbitmqStartupConfigExtensions
    {
        public static void AddRabbitMQ(
            this StartupConfig startupOption,
            Action<RabbitOptions> rabbitConfigAction,
            Func<IRabbitEventBusContainer, Task> eventBusConfig = default)
        {
            startupOption.Services.Configure<RabbitOptions>(config => rabbitConfigAction(config));
            startupOption.Services.AddSingleton<IRabbitMQClient, RabbitMQClient>();
            startupOption.Services.AddHostedService<ConsumerManager>();
            startupOption.Services.AddSingleton<IRabbitEventBusContainer, EventBusContainer>();
            startupOption.Services.AddSingleton(serviceProvider => serviceProvider.GetService<IRabbitEventBusContainer>() as IProducerContainer);
            Startup.Register(async serviceProvider =>
            {
                var container = serviceProvider.GetService<IRabbitEventBusContainer>();
                if (eventBusConfig != default)
                    await eventBusConfig(container);
                else
                    await container.AutoRegister();
            });
        }
    }
}
