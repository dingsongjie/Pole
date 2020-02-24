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
using System.Linq;

namespace Pole.EventBus.RabbitMQ
{
    public class EventBusContainer : IRabbitEventBusContainer, IProducerContainer
    {
        private readonly ConcurrentDictionary<string, RabbitEventBus> eventBusDictionary = new ConcurrentDictionary<string, RabbitEventBus>();
        private readonly List<RabbitEventBus> eventBusList = new List<RabbitEventBus>();
        readonly IRabbitMQClient rabbitMQClient;
        readonly IServiceProvider serviceProvider;
        private readonly IObserverUnitContainer observerUnitContainer;
        private readonly RabbitOptions rabbitOptions;
        public bool IsAutoRegisterFinished { get; private set; }
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
            var eventList = new List<(Type type, EventAttribute config)>();
            var evenHandlertList = new List<(Type type, EventHandlerAttribute config)>();
            AddEventAndEventHandlerInfoList(eventList, evenHandlertList);
            foreach (var (type, config) in eventList)
            {
                var eventName = config.EventName;
                var eventBus = CreateEventBus(eventName, rabbitOptions.Prefix, 1, false, true, true).BindEvent(type, eventName);
                await eventBus.AddGrainConsumer<string>();
            }
            foreach (var (type, config) in evenHandlertList)
            {
                var eventName = config.EventName;

                if (!eventBusDictionary.TryGetValue(eventName, out RabbitEventBus rabbitEventBus))
                {
                    var eventBus = CreateEventBus(eventName, rabbitOptions.Prefix, 1, false, true, true).BindEvent(type, eventName);
                    await eventBus.AddGrainConsumer<string>();
                }
            }
            IsAutoRegisterFinished = true;
        }

        public RabbitEventBus CreateEventBus(string exchange, string routePrefix, int lBCount = 1, bool autoAck = false, bool reenqueue = true, bool persistent = true)
        {
            return new RabbitEventBus(observerUnitContainer, this, exchange, routePrefix, lBCount, autoAck, reenqueue, persistent);
        }
        public Task Work(RabbitEventBus bus)
        {
            if (eventBusDictionary.TryAdd(bus.EventName, bus))
            {
                eventBusList.Add(bus);
                using var channel = rabbitMQClient.PullChannel();
                channel.Model.ExchangeDeclare($"{rabbitOptions.Prefix}{bus.Exchange}", "direct", true);
                return Task.CompletedTask;
            }
            else
                throw new EventBusRepeatException(bus.Event.FullName);
        }

        readonly ConcurrentDictionary<string, IProducer> producerDict = new ConcurrentDictionary<string, IProducer>();


        public ValueTask<IProducer> GetProducer(string typeName)
        {
            if (eventBusDictionary.TryGetValue(typeName, out var eventBus))
            {
                return new ValueTask<IProducer>(producerDict.GetOrAdd(typeName, key =>
                {
                    return new RabbitProducer(rabbitMQClient, eventBus, rabbitOptions);
                }));
            }
            else
            {
                throw new NotImplementedException($"{nameof(IProducer)} of {typeName}");
            }
        }
        public ValueTask<IProducer> GetProducer<T>()
        {
            return GetProducer(typeof(T).FullName);
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
        private void AddEventAndEventHandlerInfoList(List<(Type type, EventAttribute config)> eventList, List<(Type type, EventHandlerAttribute config)> eventHandlertList)
        {
            foreach (var assembly in AssemblyHelper.GetAssemblies(serviceProvider.GetService<ILogger<EventBusContainer>>()))
            {
                foreach (var type in assembly.GetTypes().Where(m => typeof(IEvent).IsAssignableFrom(m) && m.IsClass))
                {
                    var attribute = type.GetCustomAttributes(typeof(EventAttribute), false).FirstOrDefault();

                    if (attribute != null)
                    {
                        eventList.Add((type, (EventAttribute)attribute));
                    }
                    else
                    {
                        eventList.Add((type, new EventAttribute() { EventName = type.FullName }));
                    }
                }
            }

            foreach (var assembly in AssemblyHelper.GetAssemblies(serviceProvider.GetService<ILogger<EventBusContainer>>()))
            {

                foreach (var type in assembly.GetTypes().Where(m => typeof(IPoleEventHandler).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract&&!typeof(Orleans.Runtime.GrainReference).IsAssignableFrom(m)))
                {
                    var attribute = type.GetCustomAttributes(typeof(EventHandlerAttribute), false).FirstOrDefault();

                    if (attribute != null)
                    {
                        eventHandlertList.Add((type, (EventHandlerAttribute)attribute));
                    }
                    else
                    {
                        throw new PoleEventHandlerImplementException("Can not found EventHandlerAttribute in PoleEventHandler");
                    }
                }

            }
        }
        #endregion
    }
}
