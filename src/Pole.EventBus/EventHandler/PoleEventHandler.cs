using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Pole.EventBus.Event;
using Pole.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Emit;
using System.Linq.Expressions;
using System.Linq;
using Orleans;
using Pole.Core.Utils.Abstraction;
using Pole.EventBus.Exceptions;

namespace Pole.EventBus.EventHandler
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PoleEventHandler<TEvent> : PoleEventHandlerBase<TEvent>, IPoleEventHandler<TEvent>
    {
        public abstract Task EventHandle(TEvent @event);

    }
    public abstract class PoleBulkEventsHandler<TEvent> : PoleEventHandlerBase<TEvent>, IPoleBulkEventsHandler<TEvent>
    {
        public abstract Task BulkEventsHandle(List<TEvent> events);
    }
    public abstract class PoleEventHandlerBase<TEvent> : IPoleEventHandler
    {

        public async Task Invoke(List<EventBytesTransport> transports, ISerializer serializer, IEventTypeFinder eventTypeFinder, ILogger logger, Type eventHandlerType)
        {
            if (transports.Count() != 0)
            {
                var firstTransport = transports.First();
                var eventType = eventTypeFinder.FindType(firstTransport.EventTypeCode);
                var eventObjs = transports.Select(transport => serializer.Deserialize(firstTransport.EventBytes, eventType)).Select(@event => (TEvent)@event).ToList();
                if (this is IPoleBulkEventsHandler<TEvent> batchHandler)
                {
                    await batchHandler.BulkEventsHandle(eventObjs);
                    logger.LogTrace("Batch invoke completed: {0}->{1}->{2}", eventHandlerType.FullName, nameof(batchHandler.BulkEventsHandle), serializer.Serialize(eventObjs));
                    return;
                }
                else if (this is IPoleEventHandler<TEvent> handler)
                {
                    var handleTasks = eventObjs.Select(m => handler.EventHandle(m));
                    await Task.WhenAll(handleTasks);
                    logger.LogTrace("Invoke completed: {0}->{1}->{2}", eventHandlerType.FullName, nameof(handler.EventHandle), serializer.Serialize(eventObjs));
                    return;
                }
                else
                {
                    throw new EventHandlerImplementedNotRightException(nameof(handler.EventHandle), eventType.Name, this.GetType().FullName);
                }
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation($"{nameof(EventBytesTransport.FromBytes)} failed");
            }
        }
    }
}
