using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Pole.Domain.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Domain.EntityframeworkCore.UnitOfWork
{
    public class EntityFrameworkCoreTransactionWorker : IWorker
    {
        private readonly IDbContextTransaction _dbContextTransaction;
        public EntityFrameworkCoreTransactionWorker(DbContextOptions dbContextOptions, IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService(dbContextOptions.ContextType) as DbContext;
            _dbContextTransaction = dbContext.Database.BeginTransaction();
        }

        public int Order => 100;

        public WorkerStatus WorkerStatus { get; set; }

        public async Task Commit(CancellationToken cancellationToken = default)
        {
            await _dbContextTransaction.CommitAsync(cancellationToken);
            WorkerStatus = WorkerStatus.Commited;
        }

        public void Dispose()
        {
            // 无需手动dispose
            //_dbContextTransaction?.Dispose();
        }

        public Task PreCommit(CancellationToken cancellationToken = default)
        {
            WorkerStatus = WorkerStatus.PostCommited;
            return Task.FromResult(1);
        }

        public async Task Rollback(CancellationToken cancellationToken = default)
        {
            await _dbContextTransaction.RollbackAsync();
            WorkerStatus = WorkerStatus.Rollbacked;
        }
    }
}
