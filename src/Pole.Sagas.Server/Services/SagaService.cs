using Grpc.Core;
using Pole.Sagas.Core;
using Pole.Sagas.Server.Grpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Server.Services
{
    public class SagaService : Pole.Sagas.Server.Grpc.Saga.SagaBase
    {
        private readonly ISagaStorage sagaStorage;
        public SagaService(ISagaStorage sagaStorage)
        {
            this.sagaStorage = sagaStorage;
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
        public override async Task<CommonResponse> ActivityEnded(ActivityEndedRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                await sagaStorage.ActivityEnded(request.ActivityId, request.ResultData.ToByteArray());
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
                await sagaStorage.ActivityExecuteAborted(request.ActivityId, request.Errors);
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
                await sagaStorage.ActivityExecuteOvertime(request.ActivityId, request.SagaId, request.Errors);
                commonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                commonResponse.Errors = CombineError(ex);
            }
            return commonResponse;
        }
        public override async Task<CommonResponse> ActivityExecuteStarted(ActivityExecuteStartedRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                await sagaStorage.ActivityExecuteStarted(request.ActivityId, request.SagaId, request.TimeOutSeconds, request.ParameterData.ToByteArray(), request.Order, Convert.ToDateTime(request.AddTime));
                commonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                commonResponse.Errors = CombineError(ex);
            }
            return commonResponse;
        }
        public override async Task<CommonResponse> ActivityRetried(ActivityRetriedRequest request, ServerCallContext context)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var targetActivityRetryType = request.ActivityRetryType == Pole.Sagas.Server.Grpc.ActivityRetriedRequest.Types.ActivityRetryType.Compensate ? ActivityRetryType.Compensate : ActivityRetryType.Execute;
                await sagaStorage.ActivityRetried(request.ActivityId, request.Status, request.Retries, targetActivityRetryType);
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
                await sagaStorage.SagaEnded(request.SagaId,Convert.ToDateTime(request.ExpiresAt));
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
                await sagaStorage.SagaStarted(request.SagaId,request.ServiceName,Convert.ToDateTime( request.AddTime));
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
