using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pole.Core.Processor;
using Pole.Sagas.Core.Abstraction;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Server.Processor
{
    class PrometheusFailedSagasGaugeProcessor : IProcessor
    {
        private readonly ILogger logger;
        private readonly ISagaStorage sagaStorage;
        private readonly PoleSagasServerOption poleOptions;

        private static readonly Gauge FailedSagasGauge =
        Metrics.CreateGauge("pole_sagas_server_failed_sagas", "Pole framework sagas server failed sagas monitoring");

        public string Name => nameof(PrometheusFailedSagasGaugeProcessor);

        public PrometheusFailedSagasGaugeProcessor(
            ILogger<PrometheusFailedSagasGaugeProcessor> logger,
            ISagaStorage sagaStorage,
            IOptions<PoleSagasServerOption> poleOptions)
        {
            this.logger = logger;
            this.sagaStorage = sagaStorage;
            this.poleOptions = poleOptions.Value;
        }

        public async Task Process(ProcessingContext context)
        {
            try
            {
                logger.LogDebug($"Faild event Counter begin");

                var count = await sagaStorage.GetErrorSagasCount();
                FailedSagasGauge.Set(count);

                logger.LogDebug($"Faild event Counter ended");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(PrometheusFailedSagasGaugeProcessor)} Process Error");
            }
            finally
            {

                await Task.Delay(poleOptions.PrometheusErrorSagasGaugeIntervalSeconds * 1000);
            }
        }
    }
}
