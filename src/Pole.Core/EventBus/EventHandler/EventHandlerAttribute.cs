using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core.EventBus.EventHandler
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventHandlerAttribute: Attribute
    {
        public string EventName { get; set; }
    }
}
