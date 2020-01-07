using GreenPipes;
using GreenPipes.Configurators;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.EventBus
{
    public class MasstransitEventHandlerRegistrar
    {
        private readonly string _queueName;
        private readonly Type _eventHandlerType;
        private readonly Type _eventHandlerEventType;
        private readonly Action<IRetryConfigurator> _retryConfigure;
        public readonly ushort _prefetchCount;
        public MasstransitEventHandlerRegistrar(string eventHandlerName, Type eventHandlerType, Type eventHandlerEventType, Action<IRetryConfigurator> retryConfigure, ushort prefetchCount)
        {
            _queueName = eventHandlerName;
            _eventHandlerType = eventHandlerType;
            _eventHandlerEventType = eventHandlerEventType;
            _retryConfigure = retryConfigure;
            _prefetchCount = prefetchCount;
        }
        public void RegisterEventHandler(IServiceCollectionConfigurator serviceCollectionConfigurator, IServiceCollection services)
        {
            serviceCollectionConfigurator.AddConsumer(_eventHandlerType);
        }
        public void RegisterQueue(IServiceCollectionConfigurator serviceCollectionConfigurator, IRabbitMqBusFactoryConfigurator rabbitMqBusFactoryConfigurator, IRabbitMqHost rabbitMqHost, IServiceProvider serviceProvider)
        {

            //serviceCollectionConfigurator.AddConsumer(_eventHandlerType);

            rabbitMqBusFactoryConfigurator.ReceiveEndpoint(_queueName, conf =>
           {
               //conf.Consumer()
               conf.ConfigureConsumer(serviceProvider, _eventHandlerType);
               conf.PrefetchCount = _prefetchCount;
               conf.UseMessageRetry(_retryConfigure);
           });
        }
    }
}
