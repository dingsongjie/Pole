using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Core.EventBus.EventStorage
{
    public interface IEventStorage
    {
        Task ChangePublishStateAsync(EventEntity message, EventStatus state);
        Task ChangePublishStateAsync(IEnumerable<EventEntity> messages);
        Task BulkChangePublishStateAsync(IEnumerable<EventEntity> events);

        Task<bool> StoreMessage(EventEntity eventEntity, object dbTransaction = null);

        Task<int> DeleteExpiresAsync(string table, DateTime timeout, int batchCount = 1000,
            CancellationToken token = default);

        Task<IEnumerable<EventEntity>> GetMessagesOfNeedRetry();
    }
}
