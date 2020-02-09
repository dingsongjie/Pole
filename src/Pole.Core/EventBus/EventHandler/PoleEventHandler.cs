using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Core.EventBus.EventHandler
{
    public class PoleEventHandler : PoleEventHandlerBase
    {
        public override Task BatchInvoke(Immutable<List<byte[]>> bytes)
        {
            throw new NotImplementedException();
        }

        public override Task Invoke(Immutable<byte[]> bytes)
        {
            throw new NotImplementedException();
        }
    }
}
