﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pole.EventBus
{
    public abstract class Consumer : IConsumer
    {
        readonly List<Func<byte[], Task>> eventHandlers;
        public Consumer(
            List<Func<byte[], Task>> eventHandlers)
        {
            this.eventHandlers = eventHandlers;
        }
        public Task Notice(byte[] list)
        {
            return Task.WhenAll(eventHandlers.Select(func => func(list)));
        }
    }
}
