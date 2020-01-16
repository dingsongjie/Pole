using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Domain.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task Compelete(CancellationToken cancellationToken = default);
        Task Rollback(CancellationToken cancellationToken = default);
    }
    
}
