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
using Pole.EventBus.Abstraction;

namespace Pole.EventBus.EventHandler
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PoleEventHandler<TEvent> : PoleEventHandlerBase<TEvent>, IPoleEventHandler<TEvent> where TEvent : class,new()
    {
        public abstract Task EventHandle(TEvent @event);

    }
    public abstract class PoleEventHandlerBase<TEvent> : IPoleEventHandler where TEvent : class, new()
    {

        public async Task Invoke(EventBytesTransport transport, ISerializer serializer, IEventTypeFinder eventTypeFinder, ILogger logger, Type eventHandlerType)
        {
            var eventType = eventTypeFinder.FindType(transport.EventTypeCode);
            var @event = (TEvent)serializer.Deserialize(transport.EventBytes, eventType);

            if (this is IPoleEventHandler<TEvent> handler)
            {
                var handleTasks = handler.EventHandle(@event);
                await handleTasks;
                logger.LogTrace("Invoke completed: {0}->{1}->{2}", eventHandlerType.FullName, nameof(handler.EventHandle), serializer.Serialize(@event));
                return;
            }
            else
            {
                throw new EventHandlerImplementedNotRightException(nameof(handler.EventHandle), eventType.Name, this.GetType().FullName);
            }
        }
    }
}
