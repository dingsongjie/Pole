using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Core
{
    public interface IActivity<TData>
    {
        Task<ActivityExecuteResult> Execute(TData data);
        Task<bool> Compensate(TData data);
    }
}
