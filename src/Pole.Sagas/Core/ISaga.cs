using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Core
{
    public interface ISaga
    {
        string Id { get; }
        void AddActivity(string activityName, object data,int timeOutSeconds=2);
        Task<SagaResult> GetResult();
    }
}
