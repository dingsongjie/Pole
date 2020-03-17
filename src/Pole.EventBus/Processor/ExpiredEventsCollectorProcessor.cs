using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pole.Core;
using Pole.Core.Processor;
using Pole.EventBus.EventStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.EventBus.Processor
{
    class ExpiredEventsCollectorProcessor : IProcessor
    {
        private readonly ILogger logger;
        private readonly IEventStorageInitializer initializer;
        private readonly IEventStorage eventstorage;
        private readonly PoleEventBusOption poleOptions;

        private const int ItemBatch = 1000;
        private readonly TimeSpan _waitingInterval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);

        public string Name => nameof(ExpiredEventsCollectorProcessor);

        public ExpiredEventsCollectorProcessor(
            ILogger<ExpiredEventsCollectorProcessor> logger,
            IEventStorageInitializer initializer,
            IEventStorage eventstorage,
            IOptions<PoleEventBusOption> poleOptions)
        {
            this.logger = logger;
            this.initializer = initializer;
            this.eventstorage = eventstorage;
            this.poleOptions = poleOptions.Value;
        }

        public async Task Process(ProcessingContext context)
        {
            try
            {
                var tables = new[] { initializer.GetTableName() };

                foreach (var table in tables)
                {
                    logger.LogDebug($"Collecting expired data from table: {table}");

                    int deletedCount;
                    var time = DateTime.UtcNow;
                    do
                    {
                        deletedCount = await eventstorage.DeleteExpiresAsync(table, time, ItemBatch, context.CancellationToken);

                        if (deletedCount == ItemBatch)
                        {
                            await Task.Delay(poleOptions.ExpiredEventsPreBulkDeleteDelaySeconds * 1000);
                        }
                    } while (deletedCount == ItemBatch);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(ExpiredEventsCollectorProcessor)} Process Error");
            }
            finally
            {

                await Task.Delay(poleOptions.ExpiredEventsCollectIntervalSeconds * 1000);
            }
        }
    }
}
