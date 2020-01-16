using Microsoft.EntityFrameworkCore.Storage;
using Pole.Domain.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Domain.EntityframeworkCore.UnitOfWork
{
    public class EntityFrameworkCoreUnitOfWork : IUnitOfWork
    {
        private readonly IDbContextTransaction _dbContextTransaction;
        public EntityFrameworkCoreUnitOfWork(IDbContextTransaction dbContextTransaction)
        {
            _dbContextTransaction = dbContextTransaction;
        }
        public Task CompeleteAsync(CancellationToken cancellationToken = default)
        {
            return _dbContextTransaction.CommitAsync(cancellationToken);
        }

        public void Dispose()
        {
            _dbContextTransaction?.Dispose();
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return _dbContextTransaction.RollbackAsync();
        }
    }
}
