using Microsoft.Extensions.DependencyInjection;
using Pole.Grpc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPoleGrpc(this IServiceCollection services, params Assembly[] assemblies)
        {
            PoleGrpcOptions poleGrpcOptions = new PoleGrpcOptions(services, assemblies);
            poleGrpcOptions.AddPoleApplication();
            poleGrpcOptions.AddPoleDomain();
           // poleGrpcOptions.Services.AddGrpcValidation();
            return services;
        }
    }
}
