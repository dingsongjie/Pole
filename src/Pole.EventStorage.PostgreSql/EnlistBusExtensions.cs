using Pole.Core.EventBus;
using Pole.Core.EventBus.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pole.EventStorage.PostgreSql
{
    public static class EnlistBusExtensions
    {
        public static IDbTransaction EnlistBus(this IDbTransaction dbTransaction, IBus bus, bool autoCommit = false)
        {
            bus.Transaction = bus.ServiceProvider.GetService<IDbTransactionAdapter>();
            bus.Transaction.DbTransaction = dbTransaction;
            bus.Transaction.AutoCommit = autoCommit;
            return dbTransaction;
        }
        public static IDbContextTransaction EnlistBus(this IDbContextTransaction dbContextTransaction, IBus bus, bool autoCommit = false)
        {
            bus.Transaction = bus.ServiceProvider.GetService<IDbTransactionAdapter>();
            bus.Transaction.DbTransaction = dbContextTransaction;
            bus.Transaction.AutoCommit = autoCommit;
            return dbContextTransaction;
        }
    }
}
