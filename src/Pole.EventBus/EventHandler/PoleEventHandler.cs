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
using Pole.Core.Exceptions;
using Orleans;
using Pole.Core.Utils.Abstraction;

namespace Pole.EventBus.EventHandler
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PoleEventHandler<TEvent> : Grain
    {
        private IEventTypeFinder eventTypeFinder;
        private ISerializer serializer;
        private ILogger logger;
        private Type grainType;

        public PoleEventHandler()
        {
            grainType = GetType();
        }
        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            await DependencyInjection();
        }
        protected virtual Task DependencyInjection()
        {
            //ConfigOptions = ServiceProvider.GetOptionsByName<CoreOptions>(typeof(MainGrain).FullName);
            serializer = ServiceProvider.GetService<ISerializer>();
            eventTypeFinder = ServiceProvider.GetService<IEventTypeFinder>();
            logger = (ILogger)ServiceProvider.GetService(typeof(ILogger<>).MakeGenericType(grainType));
            return Task.CompletedTask;
        }

        public Task Invoke(EventBytesTransport transport)
        {
            var eventType = eventTypeFinder.FindType(transport.EventTypeCode);

            var eventObj = serializer.Deserialize(transport.EventBytes, eventType);
            if (this is IPoleEventHandler<TEvent> handler)
            {
                var result = handler.EventHandle((TEvent)eventObj);
                logger.LogTrace($"{nameof(PoleEventHandler<TEvent>)} Invoke completed: {0}->{1}->{2}", grainType.FullName, nameof(handler.EventHandle), serializer.Serialize(eventObj));
                return result;
            }
            else
            {
                throw new EventHandlerImplementedNotRightException(nameof(handler.EventHandle), eventType.Name, this.GetType().FullName);
            }
        }

        public async Task Invoke(List<EventBytesTransport> transports)
        {
            if (transports.Count() != 0)
            {
                var firstTransport = transports.First();
                var eventType = eventTypeFinder.FindType(firstTransport.EventTypeCode);
                var eventObjs = transports.Select(transport => serializer.Deserialize(firstTransport.EventBytes, eventType)).Select(@event => (TEvent)@event).ToList();
                if (this is IPoleBulkEventsHandler<TEvent> batchHandler)
                {
                    await batchHandler.BulkEventsHandle(eventObjs);
                    logger.LogTrace("Batch invoke completed: {0}->{1}->{2}", grainType.FullName, nameof(batchHandler.BulkEventsHandle), serializer.Serialize(eventObjs));
                    return;
                }
                else if (this is IPoleEventHandler<TEvent> handler)
                {
                    var handleTasks = eventObjs.Select(m => handler.EventHandle(m));
                    await Task.WhenAll(handleTasks);
                    logger.LogTrace("Batch invoke completed: {0}->{1}->{2}", grainType.FullName, nameof(handler.EventHandle), serializer.Serialize(eventObjs));
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
