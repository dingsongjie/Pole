using Microsoft.EntityFrameworkCore.Storage;
using Pole.Core.Serialization;
using Pole.EventBus.Transaction;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.EventStorage.PostgreSql
{
    class PostgreSqlDbTransactionAdapter : IDbTransactionAdapter
    {
        public object DbTransaction { get; set; }

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
            (DbTransaction as IDbTransaction)?.Dispose();
            DbTransaction = null;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            switch (DbTransaction)
            {
                case IDbTransaction dbTransaction:
                    dbTransaction.Rollback();
                    break;
                case IDbContextTransaction dbContextTransaction:
                    await dbContextTransaction.RollbackAsync(cancellationToken);
                    break;
            }
        }
    }
}
