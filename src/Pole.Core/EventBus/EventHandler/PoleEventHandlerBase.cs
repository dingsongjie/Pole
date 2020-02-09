using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Core.EventBus.EventHandler
{
    public abstract class PoleEventHandlerBase : Grain
    {
        public abstract Task Invoke(Immutable<byte[]> bytes);
        public abstract Task BatchInvoke(Immutable<List<byte[]>> bytes);
    }
}
