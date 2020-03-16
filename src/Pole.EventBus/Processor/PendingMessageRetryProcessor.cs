using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pole.EventBus;
using Pole.EventBus.Event;
using Pole.EventBus.EventStorage;
using Pole.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pole.Core.Processor;
using Pole.Core;
using Pole.EventBus.Abstraction;

namespace Pole.EventBus.Processor
{
    class PendingMessageRetryProcessor : ProcessorBase
    {
        private readonly IEventStorage eventStorage;
        private readonly PoleEventBusOption options;
        private readonly IProducerInfoContainer producerContainer;
        private readonly ISerializer serializer;
        private readonly ILogger<PendingMessageRetryProcessor> logger;
        private readonly IProducer producer;
        private readonly IEventBuffer eventBuffer;
        public PendingMessageRetryProcessor(IEventStorage eventStorage, IOptions<PoleEventBusOption> options, ILogger<PendingMessageRetryProcessor> logger,
            IProducerInfoContainer producerContainer, ISerializer serializer, IProducer producer, IEventBuffer eventBuffer)
        {
            this.eventStorage = eventStorage;
            this.options = options.Value ?? throw new Exception($"{nameof(PoleEventBusOption)} Must be injected");
            this.logger = logger;
            this.producerContainer = producerContainer;
            this.serializer = serializer;
            this.producer = producer;
            this.eventBuffer = eventBuffer;
        }
        public override string Name => nameof(PendingMessageRetryProcessor);


        public override async Task Process(ProcessingContext context)
        {
            try
            {
                await ProcessInternal();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(PendingMessageRetryProcessor)} Process Error");
            }
            finally
            {
                await Task.Delay(options.PendingMessageRetryIntervalSeconds * 1000);
            }
        }
        public async Task ProcessInternal()
        {
            var now = DateTime.UtcNow;
            
            var pendingMessages = await eventStorage.GetMessagesOfNeedRetry();

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug($"{nameof(PendingMessageRetryProcessor)}  pendingMessages count:{pendingMessages.Count()}");
            }
            foreach (var pendingMessage in pendingMessages)
            {
                var eventContentBytes = Encoding.UTF8.GetBytes(pendingMessage.Content);
                var bytesTransport = new EventBytesTransport(pendingMessage.Name, pendingMessage.Id, eventContentBytes);
                var bytes = bytesTransport.GetBytes();
                if (pendingMessage.Retries > options.MaxFailedRetryCount)
                {
                    pendingMessage.StatusName = nameof(EventStatus.Failed);
                    continue;
                }
                pendingMessage.Retries++;
                var targetName = producerContainer.GetTargetName(pendingMessage.Name);
                await producer.Publish(targetName, bytes);
                pendingMessage.StatusName = nameof(EventStatus.Published);
            }
            if (pendingMessages.Count() > 0)
            {
                if (pendingMessages.Count() > 10)
                {
                    await eventStorage.BulkChangePublishStateAsync(pendingMessages);
                }
                else
                {
                    await eventStorage.ChangePublishStateAsync(pendingMessages);
                }
            }
        }
    }
}
