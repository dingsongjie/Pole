using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pole.Core.EventBus
{
    public interface IObserverUnit<PrimaryKey> : IGrainID
    {
        Func<byte[], Task> GetEventHandler();
        Func<List<byte[]>, Task> GetBatchEventHandler();
    }
}
