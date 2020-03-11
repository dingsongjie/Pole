using Pole.Sagas.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Sagas.Client.Abstraction
{
    public interface IActivity<TData>
    {
        Task<ActivityExecuteResult> Execute(TData data ,CancellationToken cancellationToken);
        Task Compensate(TData data, CancellationToken cancellationToken);
    }
}
