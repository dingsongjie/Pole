using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core.EventBus.EventStorage
{
    public enum EventStatus
    {
        Failed = -1,
        PrePublish = 0,
        Published = 1
    }
}
