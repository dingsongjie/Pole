using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pole.Core.Abstraction;
using Pole.Core.EventBus;
using Pole.Core.EventBus.Event;
using Pole.Core.EventBus.EventStorage;
using Pole.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Core.Processor
{
    public class PendingMessageRetryProcessor : ProcessorBase
    {
        private readonly IEventStorage eventStorage;
        private readonly PoleOptions options;
        private readonly IProducerContainer producerContainer;
        private readonly ISerializer serializer;
        private readonly ILogger<PendingMessageRetryProcessor> logger;
        private readonly ProducerOptions producerOptions;
        public PendingMessageRetryProcessor(IEventStorage eventStorage, IOptions<PoleOptions> options, ILogger<PendingMessageRetryProcessor> logger,
            IProducerContainer producerContainer, ISerializer serializer, IOptions<ProducerOptions> producerOptions)
        {
            this.eventStorage = eventStorage;
            this.options = options.Value ?? throw new Exception($"{nameof(PoleOptions)} Must be injected");
            this.logger = logger;
            this.producerContainer = producerContainer;
            this.serializer = serializer;
            this.producerOptions = producerOptions.Value ?? throw new Exception($"{nameof(ProducerOptions)} Must be injected");
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
                if (pendingMessage.Retries > producerOptions.MaxFailedRetryCount)
                {
                    pendingMessage.ExpiresAt = DateTime.UtcNow;
                }
                pendingMessage.Retries++;
                var producer = await producerContainer.GetProducer(pendingMessage.Name);
                await producer.Publish(bytes);
                pendingMessage.StatusName = nameof(EventStatus.Published);
                pendingMessage.ExpiresAt = DateTime.UtcNow.AddSeconds(options.PublishedEventsExpiredAfterSeconds);
            }
            if (pendingMessages.Count() > 0)
            {
                await eventStorage.BulkChangePublishStateAsync(pendingMessages);
            }         
        }
    }
}
