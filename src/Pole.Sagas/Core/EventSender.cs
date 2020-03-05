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

        public Task ActivityEnded(string activityId, string resultContent)
        {
            return Task.CompletedTask;
        }

        public Task ActivityExecuteAborted(string activityId, string resultContent, string errors)
        {
            return Task.CompletedTask;
        }

        public Task ActivityRetried(string activityId, string status, int retries, string resultContent)
        {
            return Task.CompletedTask;
        }

        public Task ActivityExecuteStarted(string activityId, string sagaId, DateTime activityTimeoutTime, string parameterContent, int order)
        {
            return Task.CompletedTask;
        }

        public Task SagaEnded(string sagaId, DateTime ExpiresAt)
        {
            return Task.CompletedTask;
        }

        public Task SagaStarted(string sagaId, string serviceName)
        {
            return Task.CompletedTask;
        }
    }
}
