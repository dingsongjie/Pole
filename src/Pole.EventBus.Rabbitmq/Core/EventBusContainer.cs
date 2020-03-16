using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using RabbitMQ.Client;
using Pole.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Linq;
using Pole.EventBus.Event;
using Pole.Core.Domain;
using Pole.EventBus.EventHandler;
using Pole.EventBus.Exceptions;

namespace Pole.EventBus.RabbitMQ
{
    public class EventBusContainer : IRabbitEventBusContainer, IProducerInfoContainer
    {
        private readonly ConcurrentDictionary<string, RabbitEventBus> eventBusDictionary = new ConcurrentDictionary<string, RabbitEventBus>();
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
            var eventList = new List<(Type type, EventInfoAttribute config)>();
            var evenHandlertList = new List<(Type type, EventInfoAttribute config)>();
            AddEventAndEventHandlerInfoList(eventList, evenHandlertList);
            foreach (var (type, config) in eventList)
            {
                var eventName = config.EventName;
                var eventBus = CreateEventBus(eventName, rabbitOptions.Prefix, 1, true, true).BindEvent(type, eventName);
                await eventBus.AddGrainConsumer<string>();
            }
            foreach (var (type, config) in evenHandlertList)
            {
                var eventName = config.EventName;

                if (!eventBusDictionary.TryGetValue(eventName, out RabbitEventBus rabbitEventBus))
                {
                    var eventBus = CreateEventBus(eventName, rabbitOptions.Prefix, 1, true, true).BindEvent(type, eventName);
                    await eventBus.AddGrainConsumer<string>();
                }
            }
        }

        public RabbitEventBus CreateEventBus(string exchange, string routePrefix, int lBCount = 1, bool reenqueue = true, bool persistent = true)
        {
            return new RabbitEventBus(observerUnitContainer, this, exchange, routePrefix, lBCount, reenqueue, persistent);
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

        public string GetTargetName(string typeName)
        {
            if (eventBusDictionary.TryGetValue(typeName, out var eventBus))
            {
                return $"{rabbitOptions.Prefix}{eventBus.Exchange}";
            }
            else
            {
                throw new NotImplementedException($"{nameof(RabbitEventBus)} of {typeName}");
            }
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
        private void AddEventAndEventHandlerInfoList(List<(Type type, EventInfoAttribute config)> eventList, List<(Type type, EventInfoAttribute config)> eventHandlertList)
        {
            foreach (var assembly in AssemblyHelper.GetAssemblies(serviceProvider.GetService<ILogger<EventBusContainer>>()))
            {
                foreach (var type in assembly.GetTypes().Where(m => typeof(IEvent).IsAssignableFrom(m) && m.IsClass))
                {
                    var attribute = type.GetCustomAttributes(typeof(EventInfoAttribute), false).FirstOrDefault();

                    if (attribute != null)
                    {
                        eventList.Add((type, (EventInfoAttribute)attribute));
                    }
                    else
                    {
                        eventList.Add((type, new EventInfoAttribute() { EventName = type.FullName }));
                    }
                }
            }

            foreach (var assembly in AssemblyHelper.GetAssemblies(serviceProvider.GetService<ILogger<EventBusContainer>>()))
            {
                foreach (var type in assembly.GetTypes().Where(m => typeof(IPoleEventHandler).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract && !typeof(Orleans.Runtime.GrainReference).IsAssignableFrom(m)))
                {
                    var eventType = type.BaseType.GetGenericArguments().FirstOrDefault();
                    var attribute = eventType.GetCustomAttributes(typeof(EventInfoAttribute), false).FirstOrDefault();

                    if (attribute != null)
                    {
                        eventHandlertList.Add((type, (EventInfoAttribute)attribute));
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
