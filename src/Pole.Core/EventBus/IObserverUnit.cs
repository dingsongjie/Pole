using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pole.Core.EventBus
{
    public interface IObserverUnit<PrimaryKey> : IGrainID
    {
        List<Func<byte[], Task>> GetEventHandlers();
        List<Func<List<byte[]>, Task>> GetBatchEventHandlers();
    }
}
