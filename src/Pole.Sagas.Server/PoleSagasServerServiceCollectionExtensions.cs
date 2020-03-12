using Microsoft.Extensions.DependencyInjection;
using Pole.Core.Processor;
using Pole.Sagas.Server;
using Pole.Sagas.Server.Processor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PoleSagasServerServiceCollectionExtensions
    {
        public static IServiceCollection AddPoleSagasServer(this IServiceCollection services, Action<PoleSagasServerOption> config = null)
        {       
            Action<PoleSagasServerOption> defaultConfig = option => { };
            var finalConfig = config ?? defaultConfig;
            services.AddGrpc();
            services.Configure(config);

            services.AddSingleton<IProcessor, NotEndedSagasFetchProcessor>();
            services.AddSingleton<IProcessor, ExpiredSagasCollectorProcessor>();
            services.AddHostedService<BackgroundServiceBasedProcessorServer>();
            return services;
        }
    }
}
