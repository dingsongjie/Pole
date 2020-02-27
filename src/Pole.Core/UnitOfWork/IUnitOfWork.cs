using Pole.Core.EventBus;
using Pole.Core.EventBus.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Pole.Core.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task CompeleteAsync(CancellationToken cancellationToken = default);
        Task Rollback(CancellationToken cancellationToken = default);
        IUnitOfWork Enlist(IDbTransactionAdapter dbTransactionAdapter, IBus bus);
        IServiceProvider ServiceProvider { get; }
    }
}
