using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pole.Core.Processor;
using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Server.Processor
{
    class ExpiredSagasCollectorProcessor : ProcessorBase
    {
        private readonly ISagaStorage sagaStorage;
        private readonly PoleSagasServerOption options;
        private readonly ILogger logger;
        private readonly ISagaStorageInitializer sagaStorageInitializer;
        public ExpiredSagasCollectorProcessor(ISagaStorage sagaStorage, IOptions<PoleSagasServerOption> options, ILogger<ExpiredSagasCollectorProcessor> logger, ISagaStorageInitializer sagaStorageInitializer)
        {
            this.sagaStorage = sagaStorage;
            this.options = options.Value ?? throw new Exception($"{nameof(PoleSagasServerOption)} Must be injected");
            this.logger = logger;
            this.sagaStorageInitializer = sagaStorageInitializer;
        }
        public override string Name => nameof(NotEndedSagasFetchProcessor);


        public override async Task Process(ProcessingContext context)
        {
            try
            {
                await ProcessInternal();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(NotEndedSagasFetchProcessor)} Process Error");
            }
            finally
            {
                await Task.Delay(options.ExpiredDataBulkDeleteIntervalSeconds * 1000);
            }
        }

        private async Task ProcessInternal()
        {
            var tables = new[] { sagaStorageInitializer.GetSagaTableName() };

            foreach (var table in tables)
            {
                logger.LogDebug($"Collecting expired data from table: {table}");

                int deletedCount;
                var time = DateTime.UtcNow;
                do
                {
                    deletedCount = await sagaStorage.DeleteExpiredData(table, time, options.ExpiredDataDeleteBatchCount);

                    if (deletedCount == options.ExpiredDataDeleteBatchCount)
                    {
                        await Task.Delay(options.ExpiredDataPreBulkDeleteDelaySeconds * 1000);
                    }
                } while (deletedCount == options.ExpiredDataDeleteBatchCount);
            }
        }
    }
}
