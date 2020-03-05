using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Core
{
    public interface ISaga
    {
        string Id { get; }
        void AddActivity<TData>(string activityName, TData data);
        Task<SagaResult> GetResult();
    }
}
