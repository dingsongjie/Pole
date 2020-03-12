using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.EventBus.EventStorage
{
    public enum EventStatus
    {
        Failed = -1,
        Pending = 0,
        Published = 1
    }
}
