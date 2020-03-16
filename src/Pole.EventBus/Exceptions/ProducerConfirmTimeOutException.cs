using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.EventBus.Exceptions
{
    public class ProducerConfirmTimeOutException : Exception
    {
        public ProducerConfirmTimeOutException(int timeout) : base($"Producer wait to confirm for {timeout} seconds, timeout")
        {

        }
    }
}
