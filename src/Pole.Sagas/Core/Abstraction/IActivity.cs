using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Sagas.Core.Abstraction
{
    public interface IActivity<TData>
    {
        Task<ActivityExecuteResult> Execute(TData data ,CancellationToken cancellationToken);
        Task Compensate(TData data, CancellationToken cancellationToken);
    }
}
