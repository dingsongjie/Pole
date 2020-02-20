using Backet.Api.Domain.Event;
using Pole.Core.EventBus.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.EventHandlers
{
    public class ToNoticeBacketCreatedEventHandler : PoleEventHandler
    {
        public Task EventHandle(BacketCreatedEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
