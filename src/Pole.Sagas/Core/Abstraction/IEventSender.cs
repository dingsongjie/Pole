using System;
using System.Threading.Tasks;

namespace Pole.Sagas.Core.Abstraction
{
    public interface IEventSender
    {
        Task SagaStarted(string sagaId, string serviceName);
        Task SagaEnded(string sagaId,DateTime ExpiresAt);
        Task ActivityStarted(string activityId, string sagaId, DateTime activityTimeoutTime, string parameterContent);
        Task ActivityRetried(string activityId, string status, int retries,string resultContent);
        Task ActivityExecuteAborted(string activityId,string resultContent,string errors);
        Task ActivityCompensateAborted(string activityId,string sagaId,string errors);
        Task ActivityEnded(string activityId, string resultContent);
        Task ActivityCompensated(string activityId);
    }
}
