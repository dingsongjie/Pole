using Pole.Sagas.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Core
{
    public interface ISagaStorage
    {
        Task SagaStarted(string sagaId, string serviceName,DateTime addTime);
        Task SagaEnded(string sagaId, DateTime ExpiresAt);
        Task ActivityExecuting(string activityId, string activityName,string sagaId, byte[] ParameterData, int order,DateTime addTime,int executeTimes);
        Task ActivityExecuteAborted(string activityId);
        Task ActivityCompensateAborted(string activityId, string sagaId, string errors);
        Task ActivityExecuted(string activityId);
        Task ActivityCompensated(string activityId);
        Task ActivityExecuteOvertime(string activityId);
        Task ActivityRevoked(string activityId);
        Task ActivityCompensating(string activityId, int compensateTimes);
    }
}
