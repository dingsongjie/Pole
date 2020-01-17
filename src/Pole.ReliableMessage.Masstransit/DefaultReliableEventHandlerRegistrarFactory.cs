using Pole.ReliableMessage.EventBus;
using Pole.ReliableMessage.Masstransit.Abstraction;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pole.Domain;

namespace Pole.ReliableMessage.Masstransit
{
    class DefaultReliableEventHandlerRegistrarFactory : IReliableEventHandlerRegistrarFactory
    {
        private readonly MasstransitRabbitmqOption _masstransitOptions;
        public DefaultReliableEventHandlerRegistrarFactory(IOptions<MasstransitRabbitmqOption> masstransitOptions)
        {
            _masstransitOptions = masstransitOptions.Value ?? throw new ArgumentNullException(nameof(masstransitOptions));
        }
        public MasstransitEventHandlerRegistrar Create(Type eventHnadler)
        {

            if (!eventHnadler.Name.EndsWith(_masstransitOptions.EventHandlerNameSuffix))
            {
                throw new Exception($"EventHandler Name Must EndWith {_masstransitOptions.EventHandlerNameSuffix}");
            }
            var reliableEventHandlerParemeterAttribute = eventHnadler.GetCustomAttributes(typeof(ReliableEventHandlerParemeterAttribute), true).FirstOrDefault();

            var eventHandlerName = GetQueueName(reliableEventHandlerParemeterAttribute, eventHnadler, _masstransitOptions.QueueNamePrefix, _masstransitOptions.EventHandlerNameSuffix);

            var parentEventHandler = eventHnadler.BaseType;
            var eventType = parentEventHandler.GetGenericArguments().ToList().FirstOrDefault();

            ushort prefetchCount = GetPrefetchCount(eventHnadler, reliableEventHandlerParemeterAttribute);

            MasstransitEventHandlerRegistrar eventHandlerRegisterInvoker = new MasstransitEventHandlerRegistrar(eventHandlerName, eventHnadler, eventType, _masstransitOptions.RetryConfigure, prefetchCount);
            return eventHandlerRegisterInvoker;
        }

        private string GetQueueName(object reliableEventHandlerParemeterAttribute, Type eventHnadler, string queueNamePrefix,string eventHandlerNameSuffix)
        {
            var eventHandlerDefaultName = $"eventHandler-{ eventHnadler.Name.Replace(eventHandlerNameSuffix, "").ToLowerInvariant()}";
            var eventHandlerName = string.IsNullOrEmpty(queueNamePrefix) ? eventHandlerDefaultName : $"{queueNamePrefix}-{eventHandlerDefaultName}";

            if (reliableEventHandlerParemeterAttribute != null)
            {
                var reliableEventHandlerParemeterAttributeType = reliableEventHandlerParemeterAttribute.GetType();
                var prefetchCountPropertyInfo = reliableEventHandlerParemeterAttributeType.GetProperty(nameof(ReliableEventHandlerParemeterAttribute.QueueHaType));
                var queueHaTypeValue = Convert.ToInt32(prefetchCountPropertyInfo.GetValue(reliableEventHandlerParemeterAttribute));
                if (queueHaTypeValue != 0)
                {
                    var currentQueueType = Enumeration.FromValue<QueueHaType>(queueHaTypeValue);
                    eventHandlerName = currentQueueType.GenerateQueueName(eventHandlerName);
                }
            }

            return eventHandlerName;
        }

        private ushort GetPrefetchCount(Type eventHnadler, object reliableEventHandlerParemeterAttribute)
        {
            var prefetchCount = _masstransitOptions.PrefetchCount;
            if (reliableEventHandlerParemeterAttribute != null)
            {
                var reliableEventHandlerParemeterAttributeType = reliableEventHandlerParemeterAttribute.GetType();
                var prefetchCountPropertyInfo = reliableEventHandlerParemeterAttributeType.GetProperty(nameof(ReliableEventHandlerParemeterAttribute.PrefetchCount));
                var prefetchCountValue = Convert.ToUInt16(prefetchCountPropertyInfo.GetValue(reliableEventHandlerParemeterAttribute));
                if (prefetchCountValue != 0)
                {
                    prefetchCount = prefetchCountValue;
                }
            }

            return prefetchCount;
        }
    }
}
