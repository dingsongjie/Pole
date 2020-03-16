using Pole.EventBus.EventStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.EventBus.Abstraction
{
    public interface IEventBuffer
    {
        Task<bool> AddAndRun(EventEntity eventEntity);
    }
}
