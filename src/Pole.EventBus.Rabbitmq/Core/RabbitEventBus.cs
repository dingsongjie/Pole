using Pole.Core.Utils;
using Pole.EventBus.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pole.EventBus.RabbitMQ
{
    public class RabbitEventBus
    {
        readonly IObserverUnitContainer observerUnitContainer;
        public RabbitEventBus(
            IObserverUnitContainer observerUnitContainer,
            IRabbitEventBusContainer eventBusContainer,
            string exchange, string routePrefix, int lBCount = 1, bool reenqueue = true, bool persistent = false)
        {
            if (string.IsNullOrEmpty(exchange))
                throw new ArgumentNullException(nameof(exchange));
            if (string.IsNullOrEmpty(routePrefix))
                throw new ArgumentNullException(nameof(routePrefix));
            if (lBCount < 1)
                throw new ArgumentOutOfRangeException($"{nameof(lBCount)} must be greater than 1");
            this.observerUnitContainer = observerUnitContainer;
            Container = eventBusContainer;
            Exchange = exchange;
            RoutePrefix = routePrefix;
            LBCount = lBCount;
            Persistent = persistent;
            ConsumerConfig = new ConsumerOptions
            {
                Reenqueue = reenqueue,
                ErrorQueueSuffix = "_error",
                MaxReenqueueTimes = 10
            };
        }
        public IRabbitEventBusContainer Container { get; }
        public string Exchange { get; }
        public string RoutePrefix { get; }
        public int LBCount { get; }
        public ConsumerOptions ConsumerConfig { get; set; }
        public List<string> RouteList { get; }
        public Type Event { get; set; }
        public string EventName { get; set; }
        /// <summary>
        /// 消息是否持久化
        /// </summary>
        public bool Persistent { get; set; }
        public List<RabbitConsumer> Consumers { get; set; } = new List<RabbitConsumer>();

        public RabbitEventBus BindEvent(Type eventType, string eventName)
        {
            Event = eventType;
            EventName = eventName;
            return this;
        }
        public Task AddGrainConsumer<PrimaryKey>()
        {
            var observerUnits = observerUnitContainer.GetUnits<PrimaryKey>(EventName);
            foreach (var observerUnit in observerUnits)
            {
                string queueNameSuffix =  observerUnit.EventHandlerType.FullName;
                var consumer = new RabbitConsumer(
                    observerUnit.GetBatchEventHandler())
                {
                    EventBus = this,
                    QueueInfo = new QueueInfo { RoutingKey = string.Empty, Queue = $"{RoutePrefix}_{queueNameSuffix}" },
                    Config = ConsumerConfig
                };
                Consumers.Add(consumer);
            }
            return Enable();
        }
        public Task Enable()
        {
            return Container.Work(this);
        }
    }
}
