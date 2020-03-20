using Grpc.Net.ClientFactory;
using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Pole.Grpc.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder EnableJWTPropagation(this IHttpClientBuilder builder)
        {
            builder.Services.TryAddSingleton<JWTPropagationInterceptor>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IConfigureOptions<GrpcClientFactoryOptions>>(services =>
            {
                return new ConfigureNamedOptions<GrpcClientFactoryOptions>(builder.Name, options =>
                {
                    options.Interceptors.Add(services.GetRequiredService<JWTPropagationInterceptor>());
                });
            });
            return builder;
        }
    }
}
