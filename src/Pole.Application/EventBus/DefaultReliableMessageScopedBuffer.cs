using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pole.Application.EventBus
{
    public class DefaultReliableMessageScopedBuffer : IReliableMessageScopedBuffer
    {
        public ConcurrentBag<EventEntry> EventEntries = new ConcurrentBag<EventEntry>();
        public void Add(EventEntry eventEntry)
        {
            EventEntries.Add(eventEntry);
        }
        public IEnumerable<EventEntry> GetAll()
        {
            return EventEntries.AsEnumerable();
        }
    }
}
