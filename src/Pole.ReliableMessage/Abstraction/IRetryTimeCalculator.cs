using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IRetryTimeDelayCalculator
    {
        int Get(int retryTimes, int maxPendingMessageRetryDelay);
    }
}
