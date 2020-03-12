using Backet.Api.Domain.Event;
using Pole.EventBus.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.EventHandlers
{
    public class ToNoticeBacketCreatedEventHandler : PoleEventHandler<BacketCreatedEvent>
    {
        public override async Task EventHandle(BacketCreatedEvent @event)
        {
            await Task.Delay(1200);
        }
    }
}
