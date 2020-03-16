using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Pole.Core.UnitOfWork;
using Pole.EventBus;
using Pole.EventBus.Abstraction;
using Pole.EventBus.Transaction;
using Pole.EventBus.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Pole.Core.UnitOfWork
{
    public static class IUnitOfWorkExtensions
    {
        public static IUnitOfWork Enlist(this IUnitOfWork unitOfWork, IDbContextTransaction dbContextTransaction, IBus bus)
        {
            var dbTransactionAdapter = unitOfWork.ServiceProvider.GetRequiredService<IDbTransactionAdapter>();
            dbTransactionAdapter.DbTransaction = dbContextTransaction;
            unitOfWork.Enlist(dbTransactionAdapter, bus);
            return unitOfWork;
        }
        public static IUnitOfWork Enlist(this IUnitOfWork unitOfWork, IDbTransaction dbTransaction, IBus bus)
        {
            var dbTransactionAdapter = unitOfWork.ServiceProvider.GetRequiredService<IDbTransactionAdapter>();
            dbTransactionAdapter.DbTransaction = dbTransaction;
            unitOfWork.Enlist(dbTransactionAdapter, bus);
            return unitOfWork;
        }
    }
}
