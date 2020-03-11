using System;
using System.Threading.Tasks;

namespace Pole.Sagas.Client.Abstraction
{
    public interface IEventSender
    {
        Task SagaStarted(string sagaId, string serviceName, DateTime addTime);
        Task SagaEnded(string sagaId, DateTime ExpiresAt);
        Task ActivityExecuting(string activityId,string activityName, string sagaId, byte[] parameterData, int order, DateTime addTime);
        Task ActivityExecuteAborted(string activityId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="sagaId">sagaId 不为空 服务端会set saga.status=ended</param>
        /// <param name="errors"></param>
        /// <returns></returns>
        Task ActivityCompensateAborted(string activityId, string sagaId, string errors);
        Task ActivityCompensated(string activityId);
        Task ActivityOvertimeCompensated(string activityId,bool compensated);
        Task ActivityExecuteOvertime(string activityId,string name,byte [] parameterData,DateTime addTime);
        Task ActivityRevoked(string activityId);
    }
}
