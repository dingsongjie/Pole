using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Domain.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        //Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<CompleteResult> CompeleteAsync(CancellationToken cancellationToken = default);
    }
    
}
