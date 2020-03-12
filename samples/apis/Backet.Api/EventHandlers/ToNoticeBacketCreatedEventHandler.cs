using Backet.Api.Domain.Event;
using Backet.Api.EventHandlers.Abstraction;
using Pole.EventBus.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.EventHandlers
{
    public class ToNoticeBacketCreatedEventHandler : PoleEventHandler<BacketCreatedEvent>, IToNoticeBacketCreatedEventHandler
    {
        public async Task BulkEventsHandle(List<BacketCreatedEvent> @event)
        {          
            await Task.Delay(1500);
        }

        public async Task EventHandle(BacketCreatedEvent @event)
        {
            await Task.Delay(1200);
        }
    }
}
