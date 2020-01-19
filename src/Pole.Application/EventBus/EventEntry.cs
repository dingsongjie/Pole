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
        public Type EventType { get;private set; }
        public EventEntry(object @event,object callbackParemeter, Type eventType)
        {
            Event = @event;
            CallbackParemeter = callbackParemeter;
            EventType = eventType;
        }
    }
}
