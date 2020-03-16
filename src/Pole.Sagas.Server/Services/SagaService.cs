using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;
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
        private readonly PoleSagasServerOption poleSagasServerOption;
        public SagaService(ISagaStorage sagaStorage, ISagasBuffer sagasBuffer, IOptions<PoleSagasServerOption> poleSagasServerOption)
        {
            this.sagaStorage = sagaStorage;
            this.sagasBuffer = sagasBuffer;
            this.poleSagasServerOption = poleSagasServerOption.Value;
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
                await sagaStorage.ActivityExecuteOvertime(request.ActivityId);
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
                await sagaStorage.ActivityExecuting(request.ActivityId, request.ActivityName, request.SagaId, request.ParameterData, request.Order, Convert.ToDateTime(request.AddTime));
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
        public override async Task GetSagas(GetSagasRequest request, IServerStreamWriter<GetSagasResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                var sagaEntity = await sagasBuffer.GetSagaAvailableAsync(request.ServiceName);
                GetSagasResponse getSagasResponse = new GetSagasResponse();
                try
                {
                    //var sagaEntity =  sagasBuffer.GetSagas(request.ServiceName);
                    if (sagaEntity == null )
                    {
                        continue;
                    }
                    var saga = new GetSagasResponse.Types.Saga
                    {
                        Id = sagaEntity.Id,
                    };
                    saga.Activities.Add(sagaEntity.ActivityEntities.Select(n => new GetSagasResponse.Types.Saga.Types.Activity
                    {
                        CompensateTimes = n.CompensateTimes,
                        ExecuteTimes = n.OvertimeCompensateTimes,
                        Id = n.Id,
                        Name = n.Name,
                        Order = n.Order,
                        ParameterData = n.ParameterData,
                        SagaId = n.SagaId,
                        Status = n.Status
                    }));
                    getSagasResponse.Sagas.Add(saga);
                    getSagasResponse.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    getSagasResponse.Errors = CombineError(ex);
                }
                await responseStream.WriteAsync(getSagasResponse);
            }

        }

        public override async Task<CommonResponse> ActivityOvertimeCompensated(ActivityOvertimeCompensatedRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                await sagaStorage.ActivityOvertimeCompensated(request.ActivityId, request.Compensated);
                commonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                commonResponse.Errors = CombineError(ex);
            }
            return commonResponse;
        }

        private string CombineError(Exception exception)
        {
            return exception.InnerException != null ? exception.InnerException.Message + exception.StackTrace : exception.Message + exception.StackTrace;
        }
    }
}
