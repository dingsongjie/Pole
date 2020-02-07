using System;

namespace Pole.Core.Channels
{
    public class RebindConsumerException : Exception
    {
        public RebindConsumerException(string message) : base(message)
        {
        }
    }
}
