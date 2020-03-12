using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pole.EventBus
{
    public abstract class Consumer : IConsumer
    {
        readonly List<Func<List<byte[]>, Task>> batchEventHandlers;
        public Consumer(
            List<Func<List<byte[]>, Task>> batchEventHandlers)
        {
            this.batchEventHandlers = batchEventHandlers;
        }
        public Task Notice(List<byte[]> list)
        {
            return Task.WhenAll(batchEventHandlers.Select(func => func(list)));
        }
    }
}
