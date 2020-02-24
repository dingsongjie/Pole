using Orleans;
using Pole.Core.EventBus.Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Core.EventBus.EventHandler
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
