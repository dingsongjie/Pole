using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Domain.UnitOfWork
{
    public interface IWorker : IDisposable
    {
        int Order { get; }
        WorkerStatus WorkerStatus { get; }
        Task PreCommit(CancellationToken cancellationToken = default);
        Task Commit(CancellationToken cancellationToken = default);
        Task Rollback(CancellationToken cancellationToken = default);
    }
}
