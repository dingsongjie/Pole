using Microsoft.Extensions.DependencyInjection;
using Pole.Core.Processor;
using Pole.Sagas.Server.Processor;
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

            services.AddSingleton<IProcessor, NotEndedSagasFetchProcessor>();
            services.AddSingleton<IProcessor, ExpiredSagasCollectorProcessor>();
            services.AddHostedService<BackgroundServiceBasedProcessorServer>();
            return services;
        }
    }
}
