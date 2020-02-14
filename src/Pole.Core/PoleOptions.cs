using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core
{
    public class PoleOptions
    {
        public int PendingMessageRetryIntervalSeconds { get; set; } = 30;

        public int ExpiredEventsPreBulkDeleteDelaySeconds { get; set; } = 3;
        public int ExpiredEventsCollectIntervalSeconds { get; set; } = 60 * 60;
        public IServiceCollection Services { get; private set; }
    }
}
