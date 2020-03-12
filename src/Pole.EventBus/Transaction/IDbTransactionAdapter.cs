using Pole.EventBus.EventStorage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.EventBus.Transaction
{
    public interface IDbTransactionAdapter : IDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
        object DbTransaction { get; set; }
    }
}
