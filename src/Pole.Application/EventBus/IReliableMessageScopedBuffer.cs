using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Application.EventBus
{
    public interface IReliableMessageScopedBuffer
    {
        void Add(EventEntry eventEntry);
        IEnumerable<EventEntry> GetAll();
    }
}
