using System;
using System.Threading.Tasks;

namespace Pole.Sagas.Client.Abstraction
{
    public interface IEventSender
    {
        Task SagaStarted(string sagaId, string serviceName, DateTime addTime);
        Task SagaEnded(string sagaId, DateTime ExpiresAt);
        Task ActivityExecuting(string activityId,string activityName, string sagaId, byte[] parameterData, int order, DateTime addTime,int executeTimes);
        Task ActivityExecuteAborted(string activityId);
        Task ActivityCompensateAborted(string activityId, string sagaId, string errors);
        Task ActivityExecuted(string activityId);
        Task ActivityCompensated(string activityId);
        Task ActivityExecuteOvertime(string activityId,string name,byte [] parameterData,DateTime addTime);
        Task ActivityRevoked(string activityId);
        Task ActivityCompensating(string activityId,int compensateTimes);
    }
}
