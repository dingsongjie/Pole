using Backet.Api.Domain.Event;
using Pole.EventBus.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.EventHandlers.Abstraction
{
    public interface IToNoticeBacketCreatedEventHandler : IPoleBulkEventsHandler<BacketCreatedEvent>, IPoleEventHandler<BacketCreatedEvent>
    {
    }
}
