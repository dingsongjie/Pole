using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.EventBus
{
    public class PoleEventBusOption
    {
        public IServiceCollection Service { get; set; }
        public int PendingMessageRetryIntervalSeconds { get; set; } = 30;
        public int ExpiredEventsPreBulkDeleteDelaySeconds { get; set; } = 3;
        public int ExpiredEventsCollectIntervalSeconds { get; set; } = 10 * 60;
        public int PrometheusFaildEventGaugeIntervalSeconds { get; set; } = 30;
        public int PublishedEventsExpiredAfterSeconds { get; set; } = 60 * 60;
        public int MaxFailedRetryCount { get; set; } = 40;
    }
}
