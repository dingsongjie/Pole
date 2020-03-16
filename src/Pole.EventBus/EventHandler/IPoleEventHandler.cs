using Microsoft.Extensions.Logging;
using Orleans;
using Pole.Core.Serialization;
using Pole.EventBus.Abstraction;
using Pole.EventBus.Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.EventBus.EventHandler
{
    public interface IPoleEventHandler<TEvent> : IPoleEventHandler
    {
        Task EventHandle(TEvent @event);
    }
    public interface IPoleBulkEventsHandler<TEvent> : IPoleEventHandler
    {
        Task BulkEventsHandle(List<TEvent> events);
    }
    public interface IPoleEventHandler 
    {
        public Task Invoke(List<EventBytesTransport> transports, ISerializer serializer, IEventTypeFinder eventTypeFinder, ILogger logger, Type eventHandlerType);
    }
}
