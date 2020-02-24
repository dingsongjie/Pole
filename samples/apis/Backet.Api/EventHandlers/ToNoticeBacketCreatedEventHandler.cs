using Backet.Api.Domain.Event;
using Backet.Api.EventHandlers.Abstraction;
using Pole.Core.EventBus.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.EventHandlers
{
    [EventHandler(EventName = "Backet.Api.Domain.Event.BacketCreatedEvent")]
    public class ToNoticeBacketCreatedEventHandler : PoleEventHandler<BacketCreatedEvent>, IToNoticeBacketCreatedEventHandler
    {
        public Task BulkEventsHandle(List<BacketCreatedEvent> @event)
        {
            return Task.CompletedTask;
        }

        public Task EventHandle(BacketCreatedEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
