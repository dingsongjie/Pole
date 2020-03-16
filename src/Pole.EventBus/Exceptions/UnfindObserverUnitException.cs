using System;

namespace Pole.EventBus.Exceptions
{
    public class UnfindObserverUnitException : Exception
    {
        public UnfindObserverUnitException(string name) : base(name)
        {
        }
    }
}
