using System;

namespace Pole.EventBus.Exceptions
{
    public class UnknowTypeCodeException : Exception
    {
        public UnknowTypeCodeException(string typeName) : base(typeName)
        {
        }
    }
}
