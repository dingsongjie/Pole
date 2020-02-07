using System;

namespace Pole.Core.Exceptions
{
    public class EventBusRepeatBindingProducerException : Exception
    {
        public EventBusRepeatBindingProducerException(string name) : base(name)
        {
        }
    }
}
