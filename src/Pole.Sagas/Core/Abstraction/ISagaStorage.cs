using Google.Protobuf.Collections;
using Pole.Sagas.Core;
using Pole.Sagas.Server.Grpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Core.Abstraction
{
    public interface ISagaStorage
    {
        Task SagaStarted(string sagaId, string serviceName,DateTime addTime);
        Task SagaEnded(string sagaId, DateTime ExpiresAt);
        Task ActivityExecuting(string activityId, string activityName,string sagaId, string ParameterData, int order,DateTime addTime);
        Task ActivityExecuteAborted(string activityId);
        Task ActivityCompensateAborted(string activityId, string sagaId, string errors);
        Task ActivityCompensated(string activityId);
        Task ActivityExecuteOvertime(string activityId);
        Task ActivityRevoked(string activityId);
        IAsyncEnumerable<SagasGroupEntity> GetSagas(DateTime dateTime, int limit);
        Task<int> DeleteExpiredData(string tableName,DateTime ExpiredAt, int batchCount);
        Task ActivityOvertimeCompensated(string activityId, bool compensated);
        Task<int> GetErrorSagasCount();
    }
}
