using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using RabbitMQ.Client;
using Pole.Core.EventBus;
using Pole.Core.Exceptions;
using Pole.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pole.Core.EventBus.Event;
using Pole.Core.EventBus.EventHandler;
using Microsoft.Extensions.Options;

namespace Pole.EventBus.RabbitMQ
{
    public class EventBusContainer : IRabbitEventBusContainer, IProducerContainer
    {
        private readonly ConcurrentDictionary<Type, RabbitEventBus> eventBusDictionary = new ConcurrentDictionary<Type, RabbitEventBus>();
        private readonly List<RabbitEventBus> eventBusList = new List<RabbitEventBus>();
        readonly IRabbitMQClient rabbitMQClient;
        readonly IServiceProvider serviceProvider;
        private readonly IObserverUnitContainer observerUnitContainer;
        private readonly RabbitOptions rabbitOptions;
        public EventBusContainer(
            IServiceProvider serviceProvider,
            IObserverUnitContainer observerUnitContainer,
            IRabbitMQClient rabbitMQClient,
            IOptions<RabbitOptions> rabbitOptions)
        {
            this.serviceProvider = serviceProvider;
            this.rabbitMQClient = rabbitMQClient;
            this.observerUnitContainer = observerUnitContainer;
            this.rabbitOptions = rabbitOptions.Value;
        }
        public async Task AutoRegister()
        {
            var observableList = new List<(Type type, ProducerAttribute config)>();
            var eventList = new List<(Type type, EventAttribute config)>();
            var evenHandlertList = new List<(Type type, EventHandlerAttribute config)>();
            AddEventAndEventHandlerInfoList(eventList, evenHandlertList);
            foreach (var (type, config) in eventList)
            {
                var eventName = string.IsNullOrEmpty(config.EventName) ? type.Name.ToLower() : config.EventName;
                var eventBus = CreateEventBus(eventName, rabbitOptions.Prefix, 1, false, true, true).BindEvent(type, eventName);
                await eventBus.AddGrainConsumer<string>();
            }
            foreach (var (type, config) in evenHandlertList)
            {
                var eventName = string.IsNullOrEmpty(config.EventName) ? type.Name.ToLower() : config.EventName;
                var eventBus = CreateEventBus(eventName, rabbitOptions.Prefix, 1, false, true, true).BindEvent(type, eventName);
                await eventBus.AddGrainConsumer<string>();
            }
        }

        public RabbitEventBus CreateEventBus(string exchange, string routePrefix, int lBCount = 1, bool autoAck = false, bool reenqueue = true, bool persistent = true)
        {
            return new RabbitEventBus(observerUnitContainer, this, exchange, routePrefix, lBCount, autoAck, reenqueue, persistent);
        }
        public Task Work(RabbitEventBus bus)
        {
            if (eventBusDictionary.TryAdd(bus.Event, bus))
            {
                eventBusList.Add(bus);
                using var channel = rabbitMQClient.PullChannel();
                channel.Model.ExchangeDeclare(bus.Exchange, "direct", true);
                return Task.CompletedTask;
            }
            else
                throw new EventBusRepeatException(bus.Event.FullName);
        }

        readonly ConcurrentDictionary<Type, IProducer> producerDict = new ConcurrentDictionary<Type, IProducer>();
        public ValueTask<IProducer> GetProducer(Type type)
        {
            if (eventBusDictionary.TryGetValue(type, out var eventBus))
            {
                return new ValueTask<IProducer>(producerDict.GetOrAdd(type, key =>
                {
                    return new RabbitProducer(rabbitMQClient, eventBus);
                }));
            }
            else
            {
                throw new NotImplementedException($"{nameof(IProducer)} of {type.FullName}");
            }
        }
        public ValueTask<IProducer> GetProducer<T>()
        {
            return GetProducer(typeof(T));
        }
        public List<IConsumer> GetConsumers()
        {
            var result = new List<IConsumer>();
            foreach (var eventBus in eventBusList)
            {
                result.AddRange(eventBus.Consumers);
            }
            return result;
        }


        #region helpers
        private void AddEventAndEventHandlerInfoList(List<(Type type, EventAttribute config)> eventList, List<(Type type, EventHandlerAttribute config)> evenHandlertList)
        {
            foreach (var assembly in AssemblyHelper.GetAssemblies(serviceProvider.GetService<ILogger<EventBusContainer>>()))
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var attribute in type.GetCustomAttributes(false))
                    {
                        if (attribute is EventAttribute config)
                        {
                            eventList.Add((type, config));
                            break;
                        }
                    }
                }
            }
            foreach (var assembly in AssemblyHelper.GetAssemblies(serviceProvider.GetService<ILogger<EventBusContainer>>()))
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var attribute in type.GetCustomAttributes(false))
                    {
                        if (attribute is EventHandlerAttribute config)
                        {
                            evenHandlertList.Add((type, config));
                            break;
                        }
                    }
                }
            }
        } 
        #endregion
    }
}
