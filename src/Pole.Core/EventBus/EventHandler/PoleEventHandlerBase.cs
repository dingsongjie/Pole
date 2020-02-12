using Orleans;
using Orleans.Concurrency;
using Pole.Core.EventBus.Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Core.EventBus.EventHandler
{
    public abstract class PoleEventHandlerBase : Grain
    {
        public abstract Task Invoke(EventBytesTransport transport);
        public abstract Task Invoke(List<EventBytesTransport> transports);
    }
}
