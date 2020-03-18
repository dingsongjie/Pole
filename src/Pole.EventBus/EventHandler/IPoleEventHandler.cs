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
    public interface IPoleEventHandler<TEvent> : IPoleEventHandler where TEvent:class, new()
    {
        Task EventHandle(TEvent @event);
    }
    public interface IPoleEventHandler 
    {
        public Task Invoke(EventBytesTransport transports, ISerializer serializer, IEventTypeFinder eventTypeFinder, ILogger logger, Type eventHandlerType);
    }
}
