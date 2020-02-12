using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Pole.Core.Abstraction;
using Pole.Core.EventBus.Event;
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

namespace Pole.Core.EventBus.EventHandler
{
    /// <summary>
    /// 
    /// </summary>
    public class PoleEventHandler : PoleEventHandlerBase
    {
        private IEventTypeFinder eventTypeFinder;
        private ISerializer serializer;
        private ILogger logger;
        private Type grainType;

        public PoleEventHandler()
        {
            grainType = GetType();
            DependencyInjection();
        }
        public override async Task OnActivateAsync()
        {
            await DependencyInjection();
            await base.OnActivateAsync();
        }
        protected virtual Task DependencyInjection()
        {
            //ConfigOptions = ServiceProvider.GetOptionsByName<CoreOptions>(typeof(MainGrain).FullName);
            serializer = ServiceProvider.GetService<ISerializer>();
            eventTypeFinder = ServiceProvider.GetService<IEventTypeFinder>();
            logger = (ILogger)ServiceProvider.GetService(typeof(ILogger<>).MakeGenericType(grainType));
            return Task.CompletedTask;
        }

        public override Task Invoke(EventBytesTransport transport)
        {
            var eventType = eventTypeFinder.FindType(transport.EventTypeCode);
            var method = typeof(ClusterClientExtensions).GetMethod(Consts.EventHandlerMethodName, new Type[] { eventType });
            if (method == null)
            {
                throw new EventHandlerTargetMethodNotFoundException(Consts.EventHandlerMethodName, eventType.Name);
            }
            var data = serializer.Deserialize(transport.EventBytes, eventType);
            var eventHandlerType = this.GetType();
            var eventHandlerObjectParams = Expression.Parameter(typeof(object), "eventHandler");
            var eventHandlerParams = Expression.Convert(eventHandlerObjectParams, eventHandlerType);
            var eventObjectParams = Expression.Parameter(typeof(object), "event");
            var eventParams = Expression.Convert(eventObjectParams, eventType);

            var body = Expression.Call(method, eventHandlerParams, eventParams);
            var func = Expression.Lambda<Func<object, object, Task>>(body, true, eventHandlerObjectParams, eventObjectParams).Compile();
            var result = func(this, data);
            logger.LogTrace("Invoke completed: {0}->{1}->{2}", grainType.FullName, Consts.EventHandlerMethodName, serializer.Serialize(data));
            return result;
        }

        public override Task Invoke(List<EventBytesTransport> transports)
        {
            if (transports.Count() != 0)
            {
                var firstTransport = transports.First();
                var eventType = eventTypeFinder.FindType(firstTransport.EventTypeCode);
                var method = typeof(ClusterClientExtensions).GetMethod(Consts.BatchEventsHandlerMethodName, new Type[] { eventType });
                if (method == null)
                {
                    var tasks = transports.Select(transport => Invoke(transport));
                    return Task.WhenAll(tasks);
                }
                var datas = transports.Select(transport => serializer.Deserialize(firstTransport.EventBytes, eventType)).ToList();
                var eventHandlerType = this.GetType();
                var eventHandlerObjectParams = Expression.Parameter(typeof(object), "eventHandler");
                var eventHandlerParams = Expression.Convert(eventHandlerObjectParams, eventHandlerType);
                var eventObjectParams = Expression.Parameter(typeof(object), "events");
                var eventsType = typeof(List<>).MakeGenericType(eventType);
                var eventsParams = Expression.Convert(eventObjectParams, eventsType);

                var body = Expression.Call(method, eventHandlerParams, eventsParams);
                var func = Expression.Lambda<Func<object, object, Task>>(body, true, eventHandlerObjectParams, eventObjectParams).Compile();
                var result = func(this, datas);
                logger.LogTrace("Batch invoke completed: {0}->{1}->{2}", grainType.FullName, Consts.EventHandlerMethodName, serializer.Serialize(datas));
                return result;
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation($"{nameof(EventBytesTransport.FromBytes)} failed");
                return Task.CompletedTask;
            }
        }
    }
}
