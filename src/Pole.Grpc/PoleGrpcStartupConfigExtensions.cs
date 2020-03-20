using Microsoft.Extensions.DependencyInjection;
using Pole.Core;
using Pole.Grpc;
using Pole.Grpc.Authentication;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PoleGrpcStartupConfigExtensions
    {
        public static StartupConfig AddPoleGrpc(this StartupConfig  startupConfig, params Assembly[] assemblies)
        {
            startupConfig.Services.AddGrpcValidation();
            startupConfig.Services.AddGrpcRequestValidator();
            startupConfig.Services.AddGrpcWeb(o => o.GrpcWebEnabled = true);
            return startupConfig;
        }
    }
}
