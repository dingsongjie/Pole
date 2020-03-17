﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pole.Core.Processor;
using Pole.EventBus.EventStorage;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.EventBus.Processor
{
    class PrometheusFaildEventGaugeProcessor : IProcessor
    {
        private readonly ILogger logger;
        private readonly IEventStorage eventstorage;
        private readonly PoleEventBusOption poleOptions;

        private static readonly Gauge FaildEventGauge =
        Metrics.CreateGauge("pole_eventbus_faild_events", "Pole framework event bus faild events monitoring");

        public string Name => nameof(PrometheusFaildEventGaugeProcessor);

        public PrometheusFaildEventGaugeProcessor(
            ILogger<ExpiredEventsCollectorProcessor> logger,
            IEventStorage eventstorage,
            IOptions<PoleEventBusOption> poleOptions)
        {
            this.logger = logger;
            this.eventstorage = eventstorage;
            this.poleOptions = poleOptions.Value;
        }

        public async Task Process(ProcessingContext context)
        {
            try
            {
                logger.LogDebug($"Faild event Counter begin");

                var count = await eventstorage.GetFaildEventsCount();
                FaildEventGauge.IncTo(count);

                logger.LogDebug($"Faild event Counter ended");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(PrometheusFaildEventGaugeProcessor)} Process Error");
            }
            finally
            {

                await Task.Delay(poleOptions.PrometheusFaildEventGaugeIntervalSeconds * 1000);
            }
        }
    }
}
