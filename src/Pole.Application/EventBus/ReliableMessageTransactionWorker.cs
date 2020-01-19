using Microsoft.Extensions.Logging;
using Pole.Domain.UnitOfWork;
using Pole.ReliableMessage.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Application.EventBus
{
    public class ReliableMessageTransactionWorker : IWorker
    {
        private readonly IReliableMessageScopedBuffer _reliableMessageScopedBuffer;
        private readonly IReliableBus _reliableBus;
        private readonly ILogger<ReliableMessageTransactionWorker> _logger;

        public ReliableMessageTransactionWorker(IReliableMessageScopedBuffer reliableMessageScopedBuffer, IReliableBus reliableBus, ILogger<ReliableMessageTransactionWorker> logger)
        {
            _reliableMessageScopedBuffer = reliableMessageScopedBuffer;
            _reliableBus = reliableBus;
            _logger = logger;
        }

        public int Order => 200;

        public WorkerStatus WorkerStatus { get; set; }

        public async Task Commit(CancellationToken cancellationToken = default)
        {
            var events = _reliableMessageScopedBuffer.GetAll();
            try
            {
                var tasks = events.Select(async @event =>
                {
                    await _reliableBus.Publish(@event.Event, @event.PrePublishEventId, cancellationToken);
                });
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReliableMessageTransactionWorker.Commit error");
                // 此时 预发送成功 ,数据库事务提交成功 ,发送消息至消息队列失败 ,任然返回成功 ,因为预发送消息 的重试机制会让 消息发送成功
            }
            WorkerStatus = WorkerStatus.Commited;
            return;
        }

        public void Dispose()
        {

        }

        public async Task PreCommit(CancellationToken cancellationToken = default)
        {
            var events = _reliableMessageScopedBuffer.GetAll();
            foreach (var @event in events)
            {
                @event.PrePublishEventId = await _reliableBus.PrePublish(@event.Event, @event.EventType, @event.CallbackParemeter, cancellationToken);
            }
            WorkerStatus = WorkerStatus.PreCommited;
        }

        public Task Rollback(CancellationToken cancellationToken = default)
        {
            var events = _reliableMessageScopedBuffer.GetAll();
            events.Where(m => !string.IsNullOrEmpty(m.PrePublishEventId)).ToList().ForEach(async @event =>
            {
                await _reliableBus.Cancel(@event.PrePublishEventId, cancellationToken);
            });
            WorkerStatus = WorkerStatus.Rollbacked;
            return Task.FromResult(1);
        }
    }
}
