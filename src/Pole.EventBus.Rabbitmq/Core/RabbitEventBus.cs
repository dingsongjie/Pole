﻿using Pole.Core.EventBus;
using Pole.Core.Exceptions;
using Pole.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pole.EventBus.RabbitMQ
{
    public class RabbitEventBus
    {
        private readonly ConsistentHash _CHash;
        readonly IObserverUnitContainer observerUnitContainer;
        public RabbitEventBus(
            IObserverUnitContainer observerUnitContainer,
            IRabbitEventBusContainer eventBusContainer,
            string exchange, string routePrefix, int lBCount = 1, bool autoAck = false, bool reenqueue = true, bool persistent = false)
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
                AutoAck = autoAck,
                Reenqueue = reenqueue,
            };
            RouteList = new List<string>() { $"{routePrefix }" };
            _CHash = new ConsistentHash(RouteList, lBCount * 10);
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
        public string GetRoute(string key)
        {
            return LBCount == 1 ? RoutePrefix : _CHash.GetNode(key); ;
        }
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
                var consumer = new RabbitConsumer(
                    observerUnit.GetEventHandlers(),
                    observerUnit.GetBatchEventHandlers())
                {
                    EventBus = this,
                    QueueList = RouteList.Select(route => new QueueInfo { RoutingKey = "", Queue = $"{route}_{EventName}" }).ToList(),
                    Config = ConsumerConfig
                };
                Consumers.Add(consumer);
            }
            return Enable();
        }
        public RabbitEventBus AddConsumer(
            Func<byte[], Task> handler,
            Func<List<byte[]>, Task> batchHandler,
            string observerGroup)
        {
            var consumer = new RabbitConsumer(
                new List<Func<byte[], Task>> { handler },
                new List<Func<List<byte[]>, Task>> { batchHandler })
            {
                EventBus = this,
                QueueList = RouteList.Select(route => new QueueInfo { RoutingKey = route, Queue = $"{route}_{observerGroup}" }).ToList(),
                Config = ConsumerConfig
            };
            Consumers.Add(consumer);
            return this;
        }
        public Task Enable()
        {
            return Container.Work(this);
        }
    }
}