using Backet.Api.Grains.Abstraction;
using Backet.Grpc;
using Grpc.Core;
using Orleans;
using Pole.Grpc.ExtraType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.GrpcServices
{
    public class BacketService : Backet.Grpc.Backet.BacketBase
    {
        private readonly IClusterClient clusterClient;
        public BacketService(IClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }
        public override async Task<Pole.Grpc.ExtraType.CommonCommandResponse> AddBacket(AddBacketRequest backetDto, ServerCallContext context)
        {
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
