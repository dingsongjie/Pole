using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Server
{
    public static class PoleSagasServerApplicationBuilderExtensions
    {
        public static IApplicationBuilder UserPoleSagasServer(IApplicationBuilder builder)
        {
            builder.UseRouting();

            builder.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<BacketService>().EnableGrpcWeb();
            });

        }
    }
}
