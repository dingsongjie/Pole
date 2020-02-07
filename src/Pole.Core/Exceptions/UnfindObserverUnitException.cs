using System;

namespace Pole.Core.Exceptions
{
    public class UnfindObserverUnitException : Exception
    {
        public UnfindObserverUnitException(string name) : base(name)
        {
        }
    }
}
