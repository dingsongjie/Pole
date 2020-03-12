using Pole.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Backet.Api.Domain.Event
{
    [Pole.EventBus.Event.EventInfo(EventName = "Backet")]
    public class BacketCreatedEvent : IEvent
    {
        public string BacketId { get; set; }
    }
}
