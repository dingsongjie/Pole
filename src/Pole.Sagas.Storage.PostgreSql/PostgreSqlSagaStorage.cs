using Pole.Sagas.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Storage.PostgreSql
{
    public class PostgreSqlSagaStorage : ISagaStorage
    {
        public Task ActivityCompensateAborted(string activityId, string sagaId, string errors)
        {
            throw new NotImplementedException();
        }

        public Task ActivityCompensated(string activityId)
        {
            throw new NotImplementedException();
        }

        public Task ActivityEnded(string activityId, byte[] resultData)
        {
            throw new NotImplementedException();
        }

        public Task ActivityExecuteAborted(string activityId, string errors)
        {
            throw new NotImplementedException();
        }

        public Task ActivityExecuteOvertime(string activityId, string sagaId, string errors)
        {
            throw new NotImplementedException();
        }

        public Task ActivityExecuteStarted(string activityId, string sagaId, int timeOutSeconds, byte[] ParameterData, int order, DateTime addTime)
        {
            throw new NotImplementedException();
        }

        public Task ActivityRetried(string activityId, string status, int retries, ActivityRetryType retryType)
        {
            throw new NotImplementedException();
        }

        public Task ActivityRevoked(string activityId)
        {
            throw new NotImplementedException();
        }

        public Task SagaEnded(string sagaId, DateTime ExpiresAt)
        {
            throw new NotImplementedException();
        }

        public Task SagaStarted(string sagaId, string serviceName, DateTime addTime)
        {
            throw new NotImplementedException();
        }
    }
}
