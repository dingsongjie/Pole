using Pole.Sagas.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Core
{
    class EventSender : IEventSender
    {
        public Task ActivityCompensateAborted(string activityId, string sagaId, string errors)
        {
            return Task.CompletedTask;
        }

        public Task ActivityCompensated(string activityId)
        {
            return Task.CompletedTask;
        }

        public Task ActivityEnded(string activityId, byte[] resultData)
        {
            return Task.CompletedTask;
        }

        public Task ActivityExecuteAborted(string activityId, string errors)
        {
            return Task.CompletedTask;
        }

        public Task ActivityRetried(string activityId, string status, int retries, ActivityRetryType retryType)
        {
            return Task.CompletedTask;
        }

        public Task ActivityExecuteStarted(string activityId, string sagaId, int timeoutSeconds, byte[] ParameterData, int order, DateTime addTime)
        {
            return Task.CompletedTask;
        }

        public Task SagaEnded(string sagaId, DateTime ExpiresAt)
        {
            return Task.CompletedTask;
        }

        public Task SagaStarted(string sagaId, string serviceName, DateTime addTime)
        {
            return Task.CompletedTask;
        }

        public Task ActivityExecuteOvertime(string activityId, string sagaId, string errors)
        {
            return Task.CompletedTask;
        }

        public Task ActivityRevoked(string activityId)
        {
            throw new NotImplementedException();
        }
    }
}
