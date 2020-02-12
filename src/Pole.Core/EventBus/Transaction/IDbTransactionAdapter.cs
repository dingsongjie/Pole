using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Core.EventBus.Transaction
{
    public interface IDbTransactionAdapter : IDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
        bool AutoCommit { get; set; }
        object DbTransaction { get; set; }
    }
}
