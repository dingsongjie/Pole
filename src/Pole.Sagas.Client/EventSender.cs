using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Pole.Sagas.Client.Abstraction;
using Pole.Sagas.Core.Abstraction;
using Pole.Sagas.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Pole.Sagas.Server.Grpc.Saga;

namespace Pole.Sagas.Client
{
    public class EventSender : IEventSender
    {
        private readonly SagaClient sagaClient;
        public EventSender(SagaClient sagaClient)
        {
            this.sagaClient = sagaClient;
        }
        public async Task ActivityCompensateAborted(string activityId, string sagaId, string errors)
        {
            var result = await sagaClient.ActivityCompensateAbortedAsync(new Server.Grpc.ActivityCompensateAbortedRequest
            {
                ActivityId = activityId,
                Errors = errors,
                SagaId = sagaId
            });
            if (!result.IsSuccess)
            {
                throw new SagasServerException(result.Errors);
            }
        }

        public async Task ActivityCompensated(string activityId)
        {
            var result = await sagaClient.ActivityCompensatedAsync(new Server.Grpc.ActivityCompensatedRequest
            {
                ActivityId = activityId,
            });
            if (!result.IsSuccess)
            {
                throw new SagasServerException(result.Errors);
            }
        }

        public async Task ActivityExecuteAborted(string activityId)
        {
            var result = await sagaClient.ActivityExecuteAbortedAsync(new Server.Grpc.ActivityExecuteAbortedRequest
            {
                ActivityId = activityId
            });
            if (!result.IsSuccess)
            {
                throw new SagasServerException(result.Errors);
            }
        }

        public async Task ActivityExecuting(string activityId, string activityName, string sagaId, byte[] parameterData, int order, DateTime addTime)
        {
            var result = await sagaClient.ActivityExecutingAsync(new Server.Grpc.ActivityExecutingRequest
            {
                ActivityId = activityId,
                ActivityName = activityName,
                AddTime = addTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                Order = order,
                ParameterData = Google.Protobuf.ByteString.CopyFrom(parameterData),
                SagaId = sagaId,
            });
            if (!result.IsSuccess)
            {
                throw new SagasServerException(result.Errors);
            }
        }

        public async Task SagaEnded(string sagaId, DateTime ExpiresAt)
        {
            var result = await sagaClient.SagaEndedAsync(new Server.Grpc.SagaEndedRequest
            {
                SagaId = sagaId,
                ExpiresAt = ExpiresAt.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            });
            if (!result.IsSuccess)
            {
                throw new SagasServerException(result.Errors);
            }
        }

        public async Task SagaStarted(string sagaId, string serviceName, DateTime addTime)
        {
            var result = await sagaClient.SagaStartedAsync(new Server.Grpc.SagaStartedRequest
            {
                SagaId = sagaId,
                ServiceName = serviceName,
                AddTime = addTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            });
            if (!result.IsSuccess)
            {
                throw new SagasServerException(result.Errors);
            }
        }

        public async Task ActivityExecuteOvertime(string activityId, string name, byte[] parameterData, DateTime addTime)
        {
            var result = await sagaClient.ActivityExecuteOvertimeAsync(new Server.Grpc.ActivityExecuteOvertimeRequest
            {
                ActivityId = activityId,
            });
            if (!result.IsSuccess)
            {
                throw new SagasServerException(result.Errors);
            }
        }

        public async Task ActivityRevoked(string activityId)
        {
            var result = await sagaClient.ActivityRevokedAsync(new Server.Grpc.ActivityRevokedRequest
            {
                ActivityId = activityId,
            });
            if (!result.IsSuccess)
            {
                throw new SagasServerException(result.Errors);
            }
        }

        public async Task ActivityOvertimeCompensated(string activityId, bool compensated)
        {
            var result = await sagaClient.ActivityOvertimeCompensatedAsync(new Server.Grpc.ActivityOvertimeCompensatedRequest
            {
                ActivityId = activityId,
                Compensated= compensated
            });
            if (!result.IsSuccess)
            {
                throw new SagasServerException(result.Errors);
            }
        }
    }
}
