using Grpc.AspNetCore.Server;
using Pole.Grpc.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GrpcServiceOptionsExtensions
    {
        public static GrpcServiceOptions EnableMessageValidation(this GrpcServiceOptions options)
        {
            options.Interceptors.Add<ValidationInterceptor>();
            return options;
        }
    }
}
