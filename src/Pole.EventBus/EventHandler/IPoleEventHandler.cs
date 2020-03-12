using Orleans;
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
    public interface IPoleEventHandler : IGrainWithStringKey
    {
        public Task Invoke(EventBytesTransport transport);
        public Task Invoke(List<EventBytesTransport> transports);
    }
}
