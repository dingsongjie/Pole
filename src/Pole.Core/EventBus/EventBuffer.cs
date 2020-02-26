using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pole.Core.EventBus.Event;
using Pole.Core.EventBus.EventStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Pole.Core.EventBus
{
    class EventBuffer : IEventBuffer
    {
        readonly BufferBlock<EventEntity> buffer = new BufferBlock<EventEntity>();
        private int autoConsuming = 0;
        private readonly ILogger logger;
        /// <summary>
        /// 批量数据处理每次处理的最大数据量
        /// </summary>
        private readonly int maxBatchSize = 10000;
        /// <summary>
        /// 批量数据接收的最大延时
        /// </summary>
        private readonly int maxMillisecondsDelay = 2000;
        private readonly IProducerInfoContainer producerContainer;
        private readonly IProducer producer;
        private readonly IEventStorage eventStorage;
        private readonly PoleOptions options;
        private Task<bool> waitToReadTask;
        public EventBuffer(ILogger<EventBuffer> logger, IProducerInfoContainer producerContainer, IProducer producer, IEventStorage eventStorage, IOptions<PoleOptions> options)
        {
            this.logger = logger;
            this.producerContainer = producerContainer;
            this.producer = producer;
            this.eventStorage = eventStorage;
            this.options = options.Value;
        }
        public async Task<bool> AddAndRun(EventEntity eventEntity)
        {
            if (!buffer.Post(eventEntity))
                return await buffer.SendAsync(eventEntity);
            if (autoConsuming == 0)
                ActiveAutoExecute();

            return true;
        }
        private void ActiveAutoExecute()
        {
            if (autoConsuming == 0)
                ThreadPool.QueueUserWorkItem(ActiveConsumer);
            async void ActiveConsumer(object state)
            {
                if (Interlocked.CompareExchange(ref autoConsuming, 1, 0) == 0)
                {
                    try
                    {
                        while (await WaitToReadAsync())
                        {
                            try
                            {
                                await Execute();
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, ex.Message);
                            }
                        }
                    }
                    finally
                    {
                        Interlocked.Exchange(ref autoConsuming, 0);
                    }
                }
            }
        }
        public async Task<bool> WaitToReadAsync()
        {
            waitToReadTask = buffer.OutputAvailableAsync();
            return await waitToReadTask;

        }
        public async Task Execute()
        {
            if (waitToReadTask.IsCompletedSuccessfully && waitToReadTask.Result)
            {
                var dataList = new List<EventEntity>();
                var startTime = DateTimeOffset.UtcNow;
                while (buffer.TryReceive(out var value))
                {
                    dataList.Add(value);
                    if (dataList.Count > maxBatchSize)
                    {
                        break;
                    }
                    else if ((DateTimeOffset.UtcNow - startTime).TotalMilliseconds > maxMillisecondsDelay)
                    {
                        break;
                    }
                }
                if (dataList.Count > 0)
                {
                    await ExecuteCore(dataList);
                }

            }
        }
        private async Task ExecuteCore(List<EventEntity> eventEntities)
        {
            logger.LogError($"Begin ExecuteCore Count:{eventEntities.Count} ");
            var events = eventEntities.Select(entity =>
            {
                var eventContentBytes = Encoding.UTF8.GetBytes(entity.Content);
                var bytesTransport = new EventBytesTransport(entity.Name, entity.Id, eventContentBytes);
                var bytes = bytesTransport.GetBytes();
                var targetName = producerContainer.GetTargetName(entity.Name);
                entity.StatusName = nameof(EventStatus.Published);
                entity.ExpiresAt = DateTime.UtcNow.AddSeconds(options.PublishedEventsExpiredAfterSeconds);
                return (targetName, bytes);
            });
            eventEntities.ForEach(entity =>
            {
                entity.StatusName = nameof(EventStatus.Published);
                entity.ExpiresAt = DateTime.UtcNow.AddSeconds(options.PublishedEventsExpiredAfterSeconds);
            });
            logger.LogError($"Begin BulkPublish {DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")} ");
            await producer.BulkPublish(events);
            logger.LogError($"Begin BulkPublish {DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")} ");
            if (eventEntities.Count > 10)
            {
                await eventStorage.BulkChangePublishStateAsync(eventEntities);
            }
            else
            {
                await eventStorage.ChangePublishStateAsync(eventEntities);
            }

            logger.LogError($"End ExecuteCore {DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}  Count:{eventEntities.Count} ");
        }
    }
}
