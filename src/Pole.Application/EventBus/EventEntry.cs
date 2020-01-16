using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Application.EventBus
{
    public class EventEntry
    {
        public object Event { get;private set; }
        public object CallbackParemeter { get; private set; }
        public string PrePublishEventId { get; set; }
        public bool IsPublished { get; set; }
        public EventEntry(object @event,object callbackParemeter)
        {
            Event = @event;
            CallbackParemeter = callbackParemeter;
        }
    }
}
