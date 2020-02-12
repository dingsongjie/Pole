using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Core.EventBus
{
    public interface IBus
    {
        Task<bool> Publish(object @event, CancellationToken cancellationToken = default);
    }
}
