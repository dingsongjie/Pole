using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pole.EventBus.Transaction;

namespace Pole.EventBus.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task CompeleteAsync(CancellationToken cancellationToken = default);
        Task Rollback(CancellationToken cancellationToken = default);
        IUnitOfWork Enlist(IDbTransactionAdapter dbTransactionAdapter, IBus bus);
        IServiceProvider ServiceProvider { get; }
    }
}
