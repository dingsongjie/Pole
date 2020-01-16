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

        public ReliableMessageTransactionWorker(IReliableMessageScopedBuffer reliableMessageScopedBuffer, IReliableBus reliableBus)
        {
            _reliableMessageScopedBuffer = reliableMessageScopedBuffer;
            _reliableBus = reliableBus;
        }

        public int Order => 200;

        public WorkerStatus WorkerStatus { get; set; }

        public Task Commit(CancellationToken cancellationToken = default)
        {
            var events = _reliableMessageScopedBuffer.GetAll();
            try
            {
                events.ToList().ForEach(async @event =>
                {
                    await _reliableBus.Publish(@event.Event, @event.PrePublishEventId, cancellationToken);
                    @event.IsPublished = true;
                });
            }
            catch (Exception ex)
            {

                if (events.Count(@event => @event.IsPublished) > 1)
                {
                    //这里发布失败 通过预发送 后的重试机制去处理, 因为一旦有一个消息发出去后 无法挽回
                    return Task.FromResult(1);
                }
                else
                {
                    // 这里抛出错误 ,统一工作单元拦截后会 回滚整个工作单元
                    throw ex;
                }
            }
            WorkerStatus = WorkerStatus.Commited;
            return Task.FromResult(1);
        }

        public void Dispose()
        {

        }

        public async Task PreCommit(CancellationToken cancellationToken = default)
        {
            var events = _reliableMessageScopedBuffer.GetAll();
            foreach (var @event in events)
            {
                @event.PrePublishEventId = await _reliableBus.PrePublish(@event.Event, @event.PrePublishEventId, cancellationToken);
            }
            WorkerStatus = WorkerStatus.PreCommited;
        }

        public Task Rollback(CancellationToken cancellationToken = default)
        {
            var events = _reliableMessageScopedBuffer.GetAll();
            events.Where(m => !string.IsNullOrEmpty(m.PrePublishEventId)).ToList().ForEach(async @event =>
            {
                await _reliableBus.Cancel(@event.PrePublishEventId, cancellationToken);
                @event.IsPublished = true;
            });
            WorkerStatus = WorkerStatus.Rollbacked;
            return Task.FromResult(1);
        }
    }
}
