﻿using Backet.Api.Grains.Abstraction;
using Backet.Grpc;
using Grpc.Core;
using Orleans;
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
        public override Task<Pole.Grpc.ExtraType.CommonCommandResponse> AddBacket(AddBacketRequest backetDto, ServerCallContext context)
        {
            var newId = Guid.NewGuid().ToString("N").ToLower();
            backetDto.Id = newId;
            var grain = clusterClient.GetGrain<IAddBacketGrain>(newId);
            return Task.FromResult(Pole.Grpc.ExtraType.CommonCommandResponse.SuccessResponse);
        }
    }
}
