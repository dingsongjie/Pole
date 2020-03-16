using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.EventBus.Exceptions
{
    public class ProducerReceivedNAckException: Exception
    {
        public ProducerReceivedNAckException():base("Producer received a NAck, the broker is busy")
        {

        }
    }
}
