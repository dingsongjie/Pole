using Microsoft.Extensions.DependencyInjection;
using Pole.Core;
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
        public static StartupConfig AddSagasServer(this StartupConfig startupConfig, Action<PoleSagasServerOption> config = null)
        {
            Action<PoleSagasServerOption> defaultConfig = option => { };
            var finalConfig = config ?? defaultConfig;
            startupConfig.Services.AddGrpc();
            startupConfig.Services.Configure(finalConfig);

            startupConfig.Services.AddSingleton<IProcessor, NotEndedSagasFetchProcessor>();
            startupConfig.Services.AddSingleton<ISagasBuffer, SagasBuffer>();
            startupConfig.Services.AddSingleton<IProcessor, ExpiredSagasCollectorProcessor>();
            startupConfig.Services.AddHostedService<BackgroundServiceBasedProcessorServer>();

            return startupConfig;
        }
    }
}
