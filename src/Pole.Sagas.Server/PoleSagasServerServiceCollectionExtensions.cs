using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Server
{
    public static class PoleSagasServerServiceCollectionExtensions
    {
        public static IServiceCollection AddPoleSagasServer(IServiceCollection services)
        {
            services.AddGrpc();

            return services;
        }
    }
}
