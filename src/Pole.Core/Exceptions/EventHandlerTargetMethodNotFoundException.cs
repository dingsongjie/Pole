using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core.Exceptions
{
   public class EventHandlerImplementedNotRightException: Exception
    {
        public EventHandlerImplementedNotRightException(string methodName,string eventTypeName,string eventHandlerName):base($"EventHandler method:{methodName} errors, when eventHandler: {eventHandlerName} invoke , eventType:{eventTypeName}")
        {

        }
    }
}
