using Microsoft.AspNetCore.Builder;
using Pole.Sagas.Server.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Hosting
{
    public static class PoleSagasServerApplicationBuilderExtensions
    {
        public static IApplicationBuilder UserPoleSagasServer(this IApplicationBuilder builder)
        {
            builder.UseRouting();

            builder.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<SagaService>();
            });
            return builder;
        }
    }
}
