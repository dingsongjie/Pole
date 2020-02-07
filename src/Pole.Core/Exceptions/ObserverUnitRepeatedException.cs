using System;

namespace Pole.Core.Exceptions
{
    public class ObserverUnitRepeatedException : Exception
    {
        public ObserverUnitRepeatedException(string name) : base(name)
        {
        }
    }
}
