using Pole.Core.EventBus.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.Domain.Event
{
    public class BacketCreatedEvent:IEvent
    {
        public string BacketId { get; set; }
    }
}
