using Microsoft.EntityFrameworkCore.Storage;
using Pole.Core.EventBus.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.EventStorage.PostgreSql
{
    class PostgreSqlDbTransactionAdapter : IDbTransactionAdapter
    {
        public bool AutoCommit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public object DbTransaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            switch (DbTransaction)
            {
                case IDbTransaction dbTransaction:
                    dbTransaction.Commit();
                    break;
                case IDbContextTransaction dbContextTransaction:
                    await dbContextTransaction.CommitAsync(cancellationToken);
                    break;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
