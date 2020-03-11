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
    class NotEndedSagasFetchProcessor : ProcessorBase
    {
        private readonly ISagaStorage sagaStorage;
        private readonly PoleSagasServerOption options;
        private readonly ILogger logger;
        private readonly ISagasBuffer sagasBuffer;
        public NotEndedSagasFetchProcessor(ISagaStorage sagaStorage, IOptions<PoleSagasServerOption> options, ILogger<NotEndedSagasFetchProcessor> logger,
            ISagasBuffer sagasBuffer)
        {
            this.sagaStorage = sagaStorage;
            this.options = options.Value ?? throw new Exception($"{nameof(PoleSagasServerOption)} Must be injected");
            this.logger = logger;
            this.sagasBuffer = sagasBuffer;
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
                await Task.Delay(options.NotEndedSagasFetchIntervalSeconds * 1000);
            }
        }

        private async Task ProcessInternal()
        {
            var addTimeFilter = DateTime.UtcNow.AddMinutes(-4);
            var sagas =  sagaStorage.GetSagas(addTimeFilter, 2000);
            await sagasBuffer.AddSagas(sagas);
        }
    }
}
