using System;

namespace Pole.Core.Exceptions
{
    public class UnmatchObserverUnitException : Exception
    {
        public UnmatchObserverUnitException(string unitName) : base($"{unitName} do not match")
        {
        }
    }
}
