using Google.Protobuf;
using Grpc.Core;
using Pole.Sagas.Core;
using Pole.Sagas.Server.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Server.Services
{
    public class SagaService : Pole.Sagas.Server.Grpc.Saga.SagaBase
    {
        private readonly ISagaStorage sagaStorage;
        private readonly ISagasBuffer sagasBuffer;
        public SagaService(ISagaStorage sagaStorage, ISagasBuffer sagasBuffer)
        {
            this.sagaStorage = sagaStorage;
            this.sagasBuffer = sagasBuffer;
        }
        public override async Task<CommonResponse> ActivityCompensateAborted(ActivityCompensateAbortedRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                await sagaStorage.ActivityCompensateAborted(request.ActivityId, request.SagaId, request.Errors);
                commonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                commonResponse.Errors = CombineError(ex);
            }
            return commonResponse;
        }
        public override async Task<CommonResponse> ActivityCompensated(ActivityCompensatedRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                await sagaStorage.ActivityCompensated(request.ActivityId);
                commonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                commonResponse.Errors = CombineError(ex);
            }
            return commonResponse;
        }
        public override async Task<CommonResponse> ActivityExecuted(ActivityExecutedRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                await sagaStorage.ActivityExecuted(request.ActivityId);
                commonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                commonResponse.Errors = CombineError(ex);
            }
            return commonResponse;
        }
        public override async Task<CommonResponse> ActivityExecuteAborted(ActivityExecuteAbortedRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                await sagaStorage.ActivityExecuteAborted(request.ActivityId);
                commonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                commonResponse.Errors = CombineError(ex);
            }
            return commonResponse;
        }
        public override async Task<CommonResponse> ActivityExecuteOvertime(ActivityExecuteOvertimeRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                await sagaStorage.ActivityExecuteOvertime(request.ActivityId, request.Name, request.ParameterData.ToByteArray(), Convert.ToDateTime(request.AddTime));
                commonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                commonResponse.Errors = CombineError(ex);
            }
            return commonResponse;
        }
        public override async Task<CommonResponse> ActivityExecuting(ActivityExecutingRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                await sagaStorage.ActivityExecuting(request.ActivityId, request.ActivityName, request.SagaId, request.ParameterData.ToByteArray(), request.Order, Convert.ToDateTime(request.AddTime), request.ExecuteTimes);
                commonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                commonResponse.Errors = CombineError(ex);
            }
            return commonResponse;
        }
        public override async Task<CommonResponse> ActivityRevoked(ActivityRevokedRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                await sagaStorage.ActivityRevoked(request.ActivityId);
                commonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                commonResponse.Errors = CombineError(ex);
            }
            return commonResponse;
        }
        public override async Task<CommonResponse> SagaEnded(SagaEndedRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                await sagaStorage.SagaEnded(request.SagaId, Convert.ToDateTime(request.ExpiresAt));
                commonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                commonResponse.Errors = CombineError(ex);
            }
            return commonResponse;
        }
        public override async Task<CommonResponse> SagaStarted(SagaStartedRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                await sagaStorage.SagaStarted(request.SagaId, request.ServiceName, Convert.ToDateTime(request.AddTime));
                commonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                commonResponse.Errors = CombineError(ex);
            }
            return commonResponse;
        }
        public override async Task<CommonResponse> ActivityCompensating(ActivityCompensatingRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                await sagaStorage.ActivityCompensating(request.ActivityId, request.CompensateTimes);
                commonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                commonResponse.Errors = CombineError(ex);
            }
            return commonResponse;
        }
        public override async Task<GetSagasResponse> GetSagas(GetSagasRequest request, ServerCallContext context)
        {
            GetSagasResponse getSagasResponse = new GetSagasResponse();
            try
            {
                var sagaEntities = await sagasBuffer.GetSagas(request.ServiceName, Convert.ToDateTime(request.AddTime), request.Limit);
                var sagaDtoes = sagaEntities.Select(m =>
                {
                    var result = new GetSagasResponse.Types.Saga
                    {
                        Id = m.Id,
                    };
                    result.Activities.Add(m.ActivityEntities.Select(n => new GetSagasResponse.Types.Saga.Types.Activity
                    {
                        CompensateTimes = n.CompensateTimes,
                        ExecuteTimes = n.ExecuteTimes,
                        Id = n.Id,
                        Name = n.Id,
                        Order = n.Order,
                        ParameterData = ByteString.CopyFrom(n.ParameterData),
                        SagaId = n.SagaId,
                        Status = n.Status
                    }));
                    return result;
                });
                getSagasResponse.Sagas.Add(sagaDtoes);
                getSagasResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                getSagasResponse.Errors = CombineError(ex);
            }
            return getSagasResponse;
        }
        private string CombineError(Exception exception)
        {
            return exception.InnerException != null ? exception.InnerException.Message + exception.StackTrace : exception.Message + exception.StackTrace;
        }
    }
}
