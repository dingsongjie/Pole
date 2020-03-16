using System;

namespace Pole.EventBus.Exceptions
{
    public class ObserverUnitRepeatedException : Exception
    {
        public ObserverUnitRepeatedException(string name) : base(name)
        {
        }
    }
}
