using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core.EventBus.Event
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventInfoAttribute: Attribute
    {
        public string EventName { get; set; }
    }
}
