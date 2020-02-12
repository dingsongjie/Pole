using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core.Exceptions
{
   public class EventHandlerTargetMethodNotFoundException: Exception
    {
        public EventHandlerTargetMethodNotFoundException(string methodName,string eventTypeName):base($"EventHandler method:{methodName} not found when eventHandler invoke , eventType:{eventTypeName}")
        {

        }
    }
}
