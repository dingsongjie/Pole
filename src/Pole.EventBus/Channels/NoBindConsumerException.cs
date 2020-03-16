using System;

namespace Pole.Core.Channels
{
    public class NoBindConsumerException : Exception
    {
        public NoBindConsumerException(string message) : base(message)
        {
        }
    }
}
