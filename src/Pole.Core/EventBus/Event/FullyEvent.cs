using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core.EventBus.Event
{
    public class FullyEvent<PrimaryKey>
    {
        public IEvent Event { get; set; }
        public EventBase Base { get; set; }
        public PrimaryKey StateId { get; set; }
    }
}
