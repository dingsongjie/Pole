using Backet.Api.Grains.Abstraction;
using Backet.Grpc;
using Grpc.Core;
using Orleans;
using Pole.Grpc.ExtraType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SagasTest.Api.Test;

namespace Backet.Api.GrpcServices
{
    public class BacketService : Backet.Grpc.Backet.BacketBase
    {
        private readonly IClusterClient clusterClient;
        private readonly TestClient testClient;
        public BacketService(IClusterClient clusterClient, TestClient testClient)
        {
            this.clusterClient = clusterClient;
            this.testClient = testClient;
        }
        public override async Task<Pole.Grpc.ExtraType.CommonCommandResponse> AddBacket(AddBacketRequest backetDto, ServerCallContext context)
        {
            var result = await testClient.AddBacketAsync(new SagasTest.Api.TestRequest { });
            var newId = Guid.NewGuid().ToString("N").ToLower();
            //backetDto.Id = newId;
            var grain = clusterClient.GetGrain<IAddBacketGrain>(newId);
            //await  grain.AddBacket(backetDto);
            return Pole.Grpc.ExtraType.CommonCommandResponse.SuccessResponse;
        }
        public override async Task<CommonCommandResponse> ResponseTest(ResponseTestRequest request, ServerCallContext context)
        {
            context.Status = new Status(StatusCode.FailedPrecondition, "失败了");
            return Pole.Grpc.ExtraType.CommonCommandResponse.SuccessResponse;
        }
    }
}
