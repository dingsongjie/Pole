using System;

namespace Pole.EventBus.RabbitMQ
{
    public class EventBusRepeatException : Exception
    {
        public EventBusRepeatException(string message) : base(message)
        {
        }
    }
}
