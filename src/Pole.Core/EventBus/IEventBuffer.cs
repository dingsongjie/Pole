using Pole.Core.EventBus.EventStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Core.EventBus
{
    public interface IEventBuffer
    {
        Task<bool> AddAndRun(EventEntity eventEntity);
    }
}
